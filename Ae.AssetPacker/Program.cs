using Ae.Library;
using Ae.Library.Metadata;
using NTDLS.Helpers;
using NTDLS.SqliteDapperWrapper;
using System.CommandLine;
using System.IO.Compression;
using System.Text.Json;

namespace Ae.AssetPacker
{
    /// <summary>
    /// Used to pack a directory of assets into the database file or unpack a database file into a directory of assets.
    /// This is a simple command line tool that can be used in build scripts or manually to manage the assets in the database.
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            #region Parse command line arguments.

            var packOption = new Option<bool>("-pack")
            {
                Description = "Pack the assets from a directory into a database."
            };

            var unpackOption = new Option<bool>("-unpack")
            {
                Description = "Unpacks the assets from the database into a directory."
            };

            var pathOption = new Option<string>("-d")
            {
                Description = "Source or destination path for pack/unpack operation."
            };

            var databaseOption = new Option<string>("-db")
            {
                Description = "Location of the database file to pack/unpack."
            };

            var root = new RootCommand("Asset tool")
            {
                packOption,
                unpackOption,
                pathOption,
                databaseOption
            };

            root.Validators.Add(result =>
            {
                bool pack = result.GetValue(packOption);
                bool unpack = result.GetValue(unpackOption);
                string? path = result.GetValue(pathOption);
                string? db = result.GetValue(databaseOption);

                if (!pack && !unpack)
                {
                    result.AddError("Specify either -pack or -unpack.");
                }

                if (pack && unpack)
                {
                    result.AddError("Specify only one of -pack or -unpack, not both.");
                }

                if ((pack || unpack) && string.IsNullOrWhiteSpace(path))
                {
                    result.AddError("-o is required when using -pack or -unpack.");
                }

                if ((pack || unpack) && string.IsNullOrWhiteSpace(db))
                {
                    result.AddError("-db is required when using -pack or -unpack.");
                }
            });

            var parseResult = root.Parse(args);
            if (parseResult.Errors.Count > 0)
            {
                foreach (var error in parseResult.Errors)
                {
                    Console.WriteLine(error.Message);
                }
                return;
            }

            #endregion

            string? path = parseResult.GetValue(pathOption) ?? throw new ArgumentException("Path is required.");
            string? databasePath = parseResult.GetValue(databaseOption) ?? throw new ArgumentException("databasePath is required.");

            #region Unpack.

            if (parseResult.GetValue(unpackOption))
            {
                var database = new SqliteManagedFactory($"Data Source={databasePath}");

                var assets = database.Query<AssetDatabaseModel>(
                    "SELECT Key, BaseType, Controller, Bytes, IsCompressed, Metadata FROM Assets").ToList();

                foreach (var asset in assets)
                {
                    Console.WriteLine($"[{asset.Key}]");

                    Directory.CreateDirectory(Path.Combine(path, Path.GetDirectoryName(asset.Key) ?? string.Empty));

                    if (asset.IsCompressed)
                    {
                        File.WriteAllBytes(Path.Combine(path, asset.Key + "." + asset.BaseType), CompressionHelper.Decompress(asset.Bytes));
                    }
                    else
                    {
                        File.WriteAllBytes(Path.Combine(path, asset.Key + "." + asset.BaseType), asset.Bytes);
                    }

                    if (!string.IsNullOrWhiteSpace(asset.Metadata))
                        File.WriteAllText(Path.Combine(path, asset.Key + "." + asset.BaseType + ".meta"), asset.Metadata);

                    if (!string.IsNullOrWhiteSpace(asset.Controller))
                        File.WriteAllText(Path.Combine(path, asset.Key + "." + asset.BaseType + ".code.cs"), asset.Controller);
                }
            }

            #endregion

            #region Pack.

            string ReadIfExists(string filePath)
            {
                return File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
            }

            if (parseResult.GetValue(packOption))
            {
                if (File.Exists(databasePath))
                {
                    File.Delete(databasePath);
                }

                var database = new SqliteManagedFactory($"Data Source={databasePath}");

                database.Execute("Scripts/CreateAssetsTable.sql");

                var assetPaths = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(o => !o.EndsWith(".meta") && !o.EndsWith(".code.cs")).ToList();

                foreach (var fullAssetPath in assetPaths)
                {
                    var directory = Path.GetDirectoryName(fullAssetPath).EnsureNotNull();
                    var fileName = Path.GetFileNameWithoutExtension(fullAssetPath);
                    var relativePath = Path.GetRelativePath(path, directory);

                    var assetKey = $"{relativePath}\\{fileName}".Replace("\\", "/").Replace("//", "/");

                    var baseType = Path.GetExtension(fullAssetPath).Trim('.').ToLower();
                    var originalFileBytes = File.ReadAllBytes(fullAssetPath);
                    var compressedBytes = CompressAsset(originalFileBytes, baseType);

                    var metadataJson = ReadIfExists($"{fullAssetPath}.meta");
                    var controllerText = ReadIfExists($"{fullAssetPath}.code.cs");

                    database.Execute("DELETE FROM Assets WHERE Key = @Key", new { Key = assetKey });

                    var metadata = JsonSerializer.Deserialize<AssetMetadata>(string.IsNullOrWhiteSpace(metadataJson) ? "{}" : metadataJson, AeConstants.JsonSerializerOptions);
                    if (metadata != null)
                    {
                        metadata.AssetKey = assetKey;

                        database.Execute("INSERT INTO Assets (Key, BaseType, Bytes, IsCompressed, Metadata, Controller)"
                            + "VALUES (@Key, @BaseType, @Bytes, @IsCompressed, @Metadata, @Controller)",
                            new
                            {
                                Key = assetKey,
                                Bytes = originalFileBytes.Length > compressedBytes.Length ? compressedBytes : originalFileBytes,
                                IsCompressed = originalFileBytes.Length > compressedBytes.Length,
                                Metadata = JsonSerializer.Serialize(metadata, AeConstants.JsonSerializerOptions),
                                BaseType = baseType,
                                Controller = controllerText
                            });

                        Console.WriteLine($"[{assetKey}]");
                    }
                }
            }

            #endregion
        }

        static byte[] CompressAsset(byte[] bytes, string baseType)
        {
            if (baseType == "cs" || baseType == "txt" || baseType == "json" || baseType == "xml")
            {
                //Just for the sake of easy database editing, we do not compress text based assets.
                return bytes;
            }
            return CompressionHelper.Compress(bytes, CompressionLevel.SmallestSize);
        }
    }
}
