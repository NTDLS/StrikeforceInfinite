using Ae.Engine;
using Ae.Engine.DataModels;
using Ae.Engine.Helpers;
using NTDLS.SqliteDapperWrapper;
using System.IO.Compression;
using System.Reflection;

namespace Ae.AssetExplorer
{
    internal class ProjectExtractor
    {
        /// <summary>
        /// Extracts the assets to a buildable Visual Studio project format on disk.
        /// This is only intended for use in the editor and is not optimized for performance.
        /// </summary>
        public static string? ExtractProject(AeEngine engine, string extractPath, WriteLogDelegate writeLog)
        {
            string? foundProjectFile = null;

            var archiveBytes = AeEmbeddedResourceReader.LoadBytes("Templates/AeDebugProjectTemplate.zip");

            using var ms = new MemoryStream(archiveBytes);
            using var archive = new ZipArchive(ms, ZipArchiveMode.Read);
            var version = string.Join('.', (Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0").Split('.').Take(3));

            var assetsFullPath = Path.GetFullPath(engine.AssetPackagePath);

            Dictionary<string, string> replacements = new()
            {
                { "##PROJECT_NAME##", "Ae.Engine.Debug" },
                { "##SDK_NAME##", "Microsoft.NET.Sdk" },
                { "##DOTNET_VERSION##", "net10.0-windows" },
                { "##ASSETS_FULL_PATH##", assetsFullPath },
                { "##PACKAGE_NAME##", "Ae.Engine" },
                { "##PACKAGE_VERSION##", version }
            };

            foreach (var entry in archive.Entries)
            {
                var destinationPath = Path.Combine(extractPath, entry.FullName);

                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

                if (string.IsNullOrEmpty(entry.Name))
                    continue;

                using var entryStream = entry.Open();

                if (Path.GetExtension(entry.Name).Equals(".csproj", StringComparison.OrdinalIgnoreCase))
                {
                    if (foundProjectFile != null)
                    {
                        throw new InvalidOperationException("Multiple .csproj files found in the template. This is not supported.");
                    }

                    foundProjectFile = destinationPath;
                }

                // Handle C# files with macro replacement
                if (Path.GetExtension(entry.Name).Equals(".cs", StringComparison.OrdinalIgnoreCase)
                    || Path.GetExtension(entry.Name).Equals(".csproj", StringComparison.OrdinalIgnoreCase))
                {
                    using var reader = new StreamReader(entryStream);
                    var content = reader.ReadToEnd();

                    foreach (var replacement in replacements)
                    {
                        content = content.Replace(replacement.Key, replacement.Value);
                    }

                    File.WriteAllText(destinationPath, content);
                }
                else
                {
                    using var fileStream = File.Create(destinationPath);
                    entryStream.CopyTo(fileStream);
                }
            }
            var database = new SqliteManagedFactory($"Data Source={engine.AssetPackagePath}");

            var assets = database.Query<AssetDatabaseModel>(
                "SELECT Key, BaseType, Controller, Bytes, IsCompressed, Metadata FROM Assets").ToList();

            foreach (var asset in assets)
            {
                Directory.CreateDirectory(Path.Combine(extractPath, Path.GetDirectoryName(asset.Key) ?? string.Empty));

                if (AeConstants.BaseAssetTypes.TryGetValue(asset.BaseType, out var baseType) == false)
                {
                    continue;
                }

                if (baseType == AeBaseAssetType.Text)
                {
                    if (asset.IsCompressed)
                    {
                        File.WriteAllBytes(Path.Combine(extractPath, asset.Key + "." + asset.BaseType), CompressionHelper.Decompress(asset.Bytes));
                    }
                    else
                    {
                        File.WriteAllBytes(Path.Combine(extractPath, asset.Key + "." + asset.BaseType), asset.Bytes);
                    }
                }

                if (baseType == AeBaseAssetType.Code)
                {
                    var codeToCompile = engine.Assets.GetAssetCodeForCompilation(asset.Key, writeLog);
                    File.WriteAllText(Path.Combine(extractPath, asset.Key + "." + asset.BaseType), codeToCompile);
                }

                if (!string.IsNullOrWhiteSpace(asset.Metadata))
                    File.WriteAllText(Path.Combine(extractPath, asset.Key + "." + asset.BaseType + ".meta"), asset.Metadata);

                if (!string.IsNullOrWhiteSpace(asset.Controller))
                {
                    var codeToCompile = engine.Assets.GetAssetCodeForCompilation(asset.Key, writeLog);
                    File.WriteAllText(Path.Combine(extractPath, asset.Key + "." + asset.BaseType + ".code.cs"), codeToCompile);

                }
            }

            return foundProjectFile;
        }
    }
}
