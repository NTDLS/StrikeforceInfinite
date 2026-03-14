using Ae.Engine;
using Ae.Engine.DataModels;
using Ae.Engine.Helpers;
using Ae.Engine.Metadata;
using NTDLS.SqliteDapperWrapper;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Ae.AssetExplorer
{
    internal class ProjectMerger
    {
        private const string _aeTagsDirectory = ".ae-catalog";

        private class AeProjectFile
        {
            public string BaseType { get; set; }
            public string AssetKey { get; set; }

            /// <summary>
            /// This is the text that represents the asset when it is a text-based asset like code or json.
            /// It is what gets written to the .cs or .json file for the asset.
            /// </summary>
            public string? ObjectFile { get; set; }
            /// <summary>
            /// If the asset has metadata, it gets written to a separate file with the same name as the asset
            /// but with a .meta.json extension. This is the name of that file.
            /// </summary>
            public string? MetadataFile { get; set; }

            /// <summary>
            /// If the asset has a controller, the controller code gets written to a separate file.
            /// This is the name of that file.
            /// </summary>
            public string? ControllerFile { get; set; }

            public AeProjectFile(string baseType, string assetKey)
            {
                BaseType = baseType;
                AssetKey = assetKey;
            }
        }

        private class ExtractionCatalog
        {
            /// <summary>
            /// these are files that were extracted from the zip file and should not be re-ingested.
            /// </summary>
            public List<string> BoilerplateFiles { get; set; } = new();

            public List<AeProjectFile> AeProjectFiles { get; set; } = new();
        }

        private static readonly Regex UserCodeRegex = new(@"(?s)//\s*<ae-user-code>\s*(.*?)\s*//\s*</ae-user-code>", RegexOptions.Compiled);

        public static List<string> IngestVsProject(AeEngine engine, string projectPath, WriteLogDelegate writeLog)
        {
            var newlyAddedAssetKeys = new List<string>();

            var catalogDirectory = Path.Combine(projectPath, _aeTagsDirectory);
            var catalogJson = File.ReadAllText(Path.Combine(catalogDirectory, $"catalog.json"));
            var catalog = JsonSerializer.Deserialize<ExtractionCatalog>(catalogJson, AeConstants.JsonSerializerOptions)
                ?? throw new InvalidOperationException("Could not deserialize extraction catalog. Ingestion cannot proceed.");

            var allSourceFiles = Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories);

            foreach (var sourceFile in allSourceFiles)
            {
                var sourceFileDirectory = Path.GetDirectoryName(sourceFile);
                if (sourceFileDirectory == null)
                {
                    writeLog($"Could not determine directory for source file: {sourceFile}.", AeLoggingLevel.Warning);
                    continue;
                }

                var sourceFileName = Path.GetFileName(sourceFile);
                var relativePath = Path.GetRelativePath(projectPath, sourceFile);

                if (relativePath.Contains(_aeTagsDirectory))
                {
                    continue; //We ignore all catalog files.
                }

                if (catalog.BoilerplateFiles.Contains(relativePath, StringComparer.InvariantCultureIgnoreCase))
                {
                    continue; //We ignore all boilerplate code.
                }

                writeLog($"Ingesting asset: {relativePath}", AeLoggingLevel.Verbose);

                var filterTest = relativePath.ToLowerInvariant().Replace('/', '\\').TrimStart('\\');

                if (
                    relativePath.StartsWith(@"bin\") ||
                    relativePath.StartsWith(@"obj\") ||
                    relativePath.StartsWith(@".") || // vscode, git, packages, node_modules, dist, build, and any hidden files or folders.
                    relativePath.StartsWith(@"packages\") ||
                    relativePath.StartsWith(@"node_modules\") ||
                    relativePath.StartsWith(@"dist\") ||
                    relativePath.StartsWith(@"build\")
                )
                {
                    //Ignore build output and hidden files.
                    continue;
                }

                var objectItem = catalog.AeProjectFiles.FirstOrDefault(o => string.Equals(o.ObjectFile, relativePath, StringComparison.InvariantCultureIgnoreCase));
                if (objectItem != null)
                {
                    //Update asset object data from the project file.
                    var assetText = File.ReadAllText(sourceFile);

                    if (AeConstants.BaseAssetTypes.TryGetValue(objectItem.BaseType, out var baseType) == false)
                    {
                        writeLog($"Unknown base type for asset: {sourceFile}.", AeLoggingLevel.Warning);
                        continue;
                    }

                    if (baseType == AeBaseAssetType.Code)
                    {
                        var match = UserCodeRegex.Match(assetText); //Extract the user code section.
                        if (match.Success)
                        {
                            var userCode = match.Groups[1].Value;
                            engine.Assets.WriteAssetBytesFromText(objectItem.AssetKey, userCode);
                        }
                        else
                        {
                            writeLog($"Warning: Could not find user code section in asset file: {sourceFile}.", AeLoggingLevel.Warning);
                            engine.Assets.WriteAssetBytesFromText(objectItem.AssetKey, assetText);
                        }
                    }
                    else if (baseType == AeBaseAssetType.Text)
                    {
                        engine.Assets.WriteAssetBytesFromText(objectItem.AssetKey, assetText);
                    }
                    else
                    {
                        writeLog($"Unsupported base type for asset: {sourceFile}.", AeLoggingLevel.Warning);
                    }
                    continue;
                }

                var metadataItem = catalog.AeProjectFiles.FirstOrDefault(o => string.Equals(o.MetadataFile, relativePath, StringComparison.InvariantCultureIgnoreCase));
                if (metadataItem != null)
                {
                    //Update asset metadata from the project file.

                    var metadataJson = File.ReadAllText(sourceFile);

                    var metadata = JsonSerializer.Deserialize<AssetMetadata>(metadataJson, AeConstants.JsonSerializerOptions);
                    if (metadata == null)
                    {
                        writeLog($"Could not deserialize metadata for asset: {sourceFile}. Metadata will be skipped for this asset.", AeLoggingLevel.Warning);
                        continue;
                    }

                    engine.Assets.WriteAssetMetadata(metadataItem.AssetKey, metadata);
                    continue;
                }

                var controllerItem = catalog.AeProjectFiles.FirstOrDefault(o => string.Equals(o.ControllerFile, relativePath, StringComparison.InvariantCultureIgnoreCase));
                if (controllerItem != null)
                {
                    //Update asset controller code from the project file.
                    var controllerCode = File.ReadAllText(sourceFile);

                    var match = UserCodeRegex.Match(controllerCode); //Extract the user code section.
                    if (match.Success)
                    {
                        var userCode = match.Groups[1].Value;
                        //engine.Assets.WriteAssetControllerFromText(controllerItem.AssetKey, userCode);
                    }
                    else
                    {
                        writeLog($"Could not find user code section in controller file: {sourceFile}.", AeLoggingLevel.Warning);
                        //engine.Assets.WriteAssetControllerFromText(controllerItem.AssetKey, controllerCode);
                    }
                    continue;
                }

                var newAssetDirectory = Path.GetDirectoryName(relativePath);
                var newAssetExtension = Path.GetExtension(relativePath).Trim('.');
                var newAssetName = Path.GetFileNameWithoutExtension(relativePath).Trim('.');
                var newAssetKey = Path.Combine(newAssetDirectory ?? string.Empty, newAssetName).Replace("\\", "/");

                if (AeConstants.BaseAssetTypes.TryGetValue(newAssetExtension, out var newBaseType) == false)
                {
                    writeLog($"Unknown base type for asset: {sourceFile}.", AeLoggingLevel.Warning);
                    continue;
                }

                engine.Assets.WriteEmptyAsset(newAssetKey, newAssetExtension);
                var newAssetContent = File.ReadAllText(sourceFile) ?? string.Empty;
                engine.Assets.WriteAssetBytesFromText(newAssetKey, newAssetContent);

                newlyAddedAssetKeys.Add(newAssetKey);
            }

            return newlyAddedAssetKeys;
        }

        /// <summary>
        /// Extracts the assets to a buildable Visual Studio project format on disk.
        /// This is only intended for use in the editor and is not optimized for performance.
        /// </summary>
        public static string? ExtractVsProject(AeEngine engine, string extractPath, WriteLogDelegate writeLog)
        {
            var catalog = new ExtractionCatalog();

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

                catalog.BoilerplateFiles.Add(Path.GetRelativePath(extractPath, destinationPath));
            }

            var database = new SqliteManagedFactory($"Data Source={engine.AssetPackagePath}");

            var assets = database.Query<AssetDatabaseModel>(
                "SELECT Key, BaseType, Controller, Bytes, IsCompressed, Metadata FROM Assets").ToList();

            foreach (var asset in assets)
            {
                var projectAsset = WriteAssetProjectFile(engine, asset, extractPath, writeLog);
                if (projectAsset != null)
                {
                    catalog.AeProjectFiles.Add(projectAsset);
                }
            }

            //Write the catalog file, which contains information about which files were boilerplate and which were generated from asset
            //, as well as metadata about the assets. This allows us to know which files to ignore when ingesting changes back into the engine.
            var catalogDirectory = Path.Combine(extractPath, _aeTagsDirectory);
            Directory.CreateDirectory(catalogDirectory);
            var catalogJson = JsonSerializer.Serialize(catalog, AeConstants.JsonSerializerOptions);
            File.WriteAllText(Path.Combine(catalogDirectory, $"catalog.json"), catalogJson);

            return foundProjectFile;
        }

        private static AeProjectFile? WriteAssetProjectFile(AeEngine engine, AssetDatabaseModel asset, string extractPath, WriteLogDelegate writeLog)
        {
            var projectFile = new AeProjectFile(asset.BaseType, asset.Key);

            string thisAssetPath = Path.Combine(extractPath, Path.GetDirectoryName(asset.Key) ?? string.Empty);
            Directory.CreateDirectory(thisAssetPath);

            if (AeConstants.BaseAssetTypes.TryGetValue(asset.BaseType, out var baseType) == false)
            {
                return null;
            }

            string assetName = asset.Key.Split('/').Last();

            //This is a text, json, xml, or similar asset that we can write directly to disk.
            if (baseType == AeBaseAssetType.Text)
            {
                var objectPath = Path.Combine(thisAssetPath, $"{assetName}.{asset.BaseType}");
                projectFile.ObjectFile = Path.GetRelativePath(extractPath, objectPath);

                File.WriteAllBytes(objectPath, asset.IsCompressed ? CompressionHelper.Decompress(asset.Bytes) : asset.Bytes);
            }
            //This is a code asset, where the code is stored in the asset bytes.
            else if (baseType == AeBaseAssetType.Code)
            {
                var objectPath = Path.Combine(thisAssetPath, $"{assetName}.{asset.BaseType}");
                projectFile.ObjectFile = Path.GetRelativePath(extractPath, objectPath);

                var codeToCompile = engine.Assets.GetAssetCodeForCompilation(asset.Key, writeLog) ?? string.Empty;
                File.WriteAllText(objectPath, codeToCompile);
            }
            else
            {
                //We do not write out sound and image assets for the debug project, as they are not needed and can be large.
                projectFile.ObjectFile = null;
            }

            //Write the metadata to a separate file.
            if (!string.IsNullOrWhiteSpace(asset.Metadata))
            {
                var metadataPath = Path.Combine(thisAssetPath, $"{assetName}.{asset.BaseType}.meta.json");
                projectFile.MetadataFile = Path.GetRelativePath(extractPath, metadataPath);
                File.WriteAllText(metadataPath, asset.Metadata);
            }

            //If the asset has a controller, write the controller code to a separate file.
            if (!string.IsNullOrWhiteSpace(asset.Controller))
            {
                var controllerPath = Path.Combine(thisAssetPath, $"{assetName}.{asset.BaseType}.controller.cs");
                projectFile.ControllerFile = Path.GetRelativePath(extractPath, controllerPath);
                var codeToCompile = engine.Assets.GetAssetCodeForCompilation(asset.Key, writeLog);
                File.WriteAllText(controllerPath, codeToCompile);
            }

            return projectFile;
        }
    }
}
