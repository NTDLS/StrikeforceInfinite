using Ae.Engine.Audio;
using Ae.Engine.Compiler;
using Ae.Engine.DataModels;
using Ae.Engine.Helpers;
using Ae.Engine.Metadata;
using NTDLS.DelegateThreadPooling;
using NTDLS.SqliteDapperWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Manager
{
    /// <summary>
    /// Provides functionality for loading, managing, and accessing assets within the application. Supports asset
    /// retrieval, metadata access, and asset modification operations, including reading, writing, and deleting assets
    /// from the underlying database.
    /// </summary>
    /// <remarks>The asset manager maintains an in-memory collection of assets and uses caching to optimize
    /// asset queries. Asset loading and modification operations update both the database and the in-memory collection.
    /// Thread safety is ensured during asset collection updates. The manager supports various asset types, including
    /// text, images, audio, and code, and provides specialized methods for common asset retrieval scenarios. Asset
    /// modification methods are primarily intended for use in editor contexts.</remarks>
    public class AssetManager
    {
        /// <summary>
        /// Gets a value indicating whether the asset package has been successfully loaded.
        /// </summary>
        public bool IsLoaded { get; private set; }
        /// <summary>
        /// Gets the engine instance used to execute and manage workflow operations.
        /// </summary>
        public AeEngine Engine { get; private set; }
        private readonly Dictionary<string, AssetContainer> _collection = new();
        private readonly SqliteManagedFactory _assetsDatabase;
        private readonly AeCache _cache = new(AeCache.CacheExpirationScheme.Sliding, TimeSpan.FromSeconds(600));

        /// <summary>
        /// Initializes a new instance of the AssetManager class using the specified engine.
        /// </summary>
        /// <param name="engine">The engine instance used to manage assets. Cannot be null.</param>
        public AssetManager(AeEngine engine)
        {
            Engine = engine;
            _assetsDatabase = new($"Data Source={engine.AssetPackagePath}");
        }

        /// <summary>
        /// Gets the metadata for all assets in a directory.
        /// This REQUIRES that the assets already be cached.
        /// </summary>
        public List<string> GetAssetKeysInPath(string path)
            => _cache.AddOrGet($"GetAssetKeysInPath:{path}", () =>
                _collection.Where(kv => kv.Key.StartsWith(path, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Key).ToList()) ?? [];

        /// <summary>
        /// Gets the metadata for all assets in a directory.
        /// This REQUIRES that the assets already be cached.
        /// </summary>
        public List<AssetContainer> GetAssetsInPath(string path)
            => _cache.AddOrGet($"GetAssetsInPath:{path}", () =>
            _collection.Where(kv => kv.Key.StartsWith(path, StringComparison.OrdinalIgnoreCase)).Select(kv => kv.Value).ToList()) ?? [];

        /// <summary>
        /// Gets the metadata for all assets.
        /// This REQUIRES that the assets already be cached.
        /// </summary>
        public List<AssetContainer> GetAssets()
            => _collection.Values.ToList();

        /// <summary>
        /// Retrieves the asset container associated with the specified asset key.
        /// </summary>
        /// <param name="assetKey">The unique key identifying the asset to retrieve. Cannot be null or empty.</param>
        /// <returns>The asset container corresponding to the specified asset key.</returns>
        /// <exception cref="FileNotFoundException">Thrown if no asset container exists for the specified asset key.</exception>
        public AssetContainer GetAsset(string assetKey)
        {
            if (_collection.TryGetValue(assetKey, out AssetContainer? assetContainer))
            {
                return assetContainer;
            }
            throw new FileNotFoundException($"Asset not found: {assetKey}");
        }

        /// <summary>
        /// Retrieves the metadata associated with the specified asset key.
        /// </summary>
        /// <param name="assetKey">The unique key identifying the asset whose metadata is to be retrieved. Cannot be null or empty.</param>
        /// <returns>The metadata for the asset identified by the specified key.</returns>
        /// <exception cref="FileNotFoundException">Thrown if no asset exists for the specified key.</exception>
        public AssetMetadata GetMetadata(string assetKey)
        {
            if (_collection.TryGetValue(assetKey, out AssetContainer? assetContainer))
            {
                return assetContainer.Metadata;
            }
            throw new FileNotFoundException($"Asset not found: {assetKey}");
        }

        /// <summary>
        /// Retrieves the text content associated with the specified asset key.
        /// </summary>
        /// <param name="assetKey">The unique identifier for the asset whose text content is to be retrieved. Cannot be null or empty.</param>
        /// <returns>A string containing the text content of the asset associated with the specified key.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the asset is not found or cannot be converted to text.</exception>
        public string GetText(string assetKey)
        {
            if (_collection.TryGetValue(assetKey, out AssetContainer? assetContainer))
            {
                return assetContainer.Object as string
                    ?? throw new FileNotFoundException($"Asset could not be converted to text: {assetKey}");
            }
            throw new FileNotFoundException($"Asset not found: {assetKey}");
        }

        /// <summary>
        /// Retrieves an audio clip associated with the specified asset key.
        /// </summary>
        /// <remarks>The returned audio clip's volume and looping settings are initialized based on the
        /// asset's metadata. If the asset is not found or is not an audio clip, a FileNotFoundException is
        /// thrown.</remarks>
        /// <param name="assetKey">The unique identifier for the audio asset to retrieve. Cannot be null or empty.</param>
        /// <returns>An instance of AeAudioClip representing the requested audio asset. The clip will have its initial volume and
        /// looping behavior set according to the asset's metadata.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the asset with the specified key does not exist or cannot be converted to an audio clip.</exception>
        public AeAudioClip GetAudio(string assetKey)
        {
            if (_collection.TryGetValue(assetKey, out AssetContainer? assetContainer))
            {
                var audioClip = assetContainer.Object as AeAudioClip
                    ?? throw new FileNotFoundException($"Asset could not be converted to audio: {assetKey}");
                audioClip.SetInitialVolume(assetContainer.Metadata.SoundVolume ?? 1);
                audioClip.SetLoopForever(assetContainer.Metadata.LoopSound ?? false);
                return audioClip;
            }
            throw new FileNotFoundException($"Asset not found: {assetKey}");
        }

        /// <summary>
        /// Retrieves the bitmap asset associated with the specified asset key.
        /// </summary>
        /// <param name="assetKey">The unique key identifying the asset to retrieve. Cannot be null or empty.</param>
        /// <returns>The bitmap asset corresponding to the specified asset key.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the asset is not found or cannot be converted to a bitmap.</exception>
        public SharpDX.Direct2D1.Bitmap GetBitmap(string assetKey)
        {
            if (_collection.TryGetValue(assetKey, out AssetContainer? assetContainer))
            {
                return assetContainer.Object as SharpDX.Direct2D1.Bitmap
                    ?? throw new FileNotFoundException($"Asset could not be converted to bitmap: {assetKey}");
            }
            throw new FileNotFoundException($"Asset not found: {assetKey}");
        }

        /// <summary>
        /// Retrieves the source code for the specified asset, formatted for runtime compilation.
        /// </summary>
        /// <remarks>The returned code is suitable for runtime compilation and depends on the asset's base
        /// type. For code assets, the source is extracted from the asset's bytes; for non-code assets, the controller
        /// field is used. If the asset cannot be compiled, null is returned.</remarks>
        /// <param name="assetKey">The unique key identifying the asset whose code is to be retrieved. Cannot be null or empty.</param>
        /// <param name="writeLog">An optional delegate used to log errors or informational messages during asset retrieval. If not provided,
        /// exceptions will be thrown for unsupported asset types.</param>
        /// <returns>A string containing the asset's source code formatted for compilation, or null if the asset cannot be
        /// compiled.</returns>
        /// <exception cref="Exception">Thrown if the asset is not found, the asset metadata does not contain a valid AssetKey, or the asset base
        /// type is unsupported and no log delegate is provided.</exception>
        public string? GetAssetCodeForCompilation(string assetKey, WriteLogDelegate? writeLog = null)
        {
            var model = _assetsDatabase.QueryFirstOrDefault<AssetDatabaseModel>(
                "SELECT Key, BaseType, Controller, Bytes, IsCompressed, Metadata FROM Assets WHERE Key = @Key", new { Key = assetKey })
                ?? throw new Exception($"Asset not found {assetKey}");

            var assetContainer = DeserializeAssetContainer(model);

            if (assetContainer.Metadata.AssetKey == null)
            {
                throw new Exception($"Asset metadata for asset with key: {assetContainer.Key} does not contain a valid AssetKey.");
            }

            if (BaseAssetTypes.TryGetValue(model.BaseType, out var baseType) == false)
            {
                if (writeLog != null)
                {
                    writeLog($"Unsupported asset base type: {model.BaseType} for asset with key: {model.Key}", AeLoggingLevel.Error, assetKey);
                }
                else throw new Exception($"Unsupported asset base type: {model.BaseType} for asset with key: {model.Key}");
            }

            var friendlyName = assetContainer.Metadata.AssetKey.Split('/').LastOrDefault()
                ?? throw new Exception($"Asset metadata for asset with key: {assetContainer.Key} does not contain a valid AssetKey.");

            string? assetDynamicCode = null;
            Type? interfaceType = null;

            if (baseType != AeBaseAssetType.Code
                && !string.IsNullOrWhiteSpace(assetContainer.Metadata.Class)
                && !string.IsNullOrWhiteSpace(model.Controller))
            {
                //Non-code ("sprite") asset code is the Controller field and Metadata.Class is the base class.
                assetDynamicCode = model.Controller;
                interfaceType = typeof(IAeRuntimeCompiledSpriteAsset);
            }
            else if (baseType == AeBaseAssetType.Code)
            {
                //"Code" asset type code is in the Bytes field.
                assetDynamicCode = Encoding.UTF8.GetString(model.Bytes);
                interfaceType = typeof(IAeRuntimeCompiledCodeAsset);
            }

            if (assetDynamicCode != null && interfaceType != null)
            {
                //Compile user code for asset.
                return AeAssetCodeClassBuilder.Get(
                    assetContainer.Metadata.Class, assetContainer.Metadata.DynamicTypeName, assetDynamicCode, interfaceType, friendlyName);
            }

            return null;
        }

        internal void LoadAllAssets(Action<string, float>? progressCallback, WriteLogDelegate? writeLog = null)
        {
            progressCallback?.Invoke("Loading assets...", 0);

            using var dtp = new DelegateThreadPool(new DelegateThreadPoolConfiguration()
            {
                InitialThreadCount = Environment.ProcessorCount * 4,
                MaximumThreadCount = Environment.ProcessorCount * 4,
            });
            var threadPoolTracker = dtp.CreateChildPool();

            var models = _assetsDatabase.Query<AssetDatabaseModel>(
                "SELECT Key, BaseType, Controller, Bytes, IsCompressed, Metadata FROM Assets WHERE BaseType = 'cs'").ToList();

            models.AddRange(_assetsDatabase.Query<AssetDatabaseModel>(
                "SELECT Key, BaseType, Controller, Bytes, IsCompressed, Metadata FROM Assets WHERE BaseType != 'cs'"));

            int statusIndex = 0;
            float statusEntryCount = models.Count();

            foreach (var model in models)
            {
                threadPoolTracker.Enqueue(() =>
                {
                    if (BaseAssetTypes.TryGetValue(model.BaseType, out var baseType) == false)
                    {
                        if (writeLog != null)
                        {
                            writeLog($"Unsupported asset base type: {model.BaseType} for asset with key: {model.Key}", AeLoggingLevel.Error, model.Key);
                        }
                        else throw new Exception($"Unsupported asset base type: {model.BaseType} for asset with key: {model.Key}");
                    }

                    //AeAssetCodeClassText
                    var assetContainer = DeserializeAssetContainer(model);
                    if (string.IsNullOrEmpty(assetContainer.Metadata.AssetKey))
                    {
                        if (writeLog != null)
                        {
                            writeLog($"Asset metadata for asset with key: {model.Key} does not contain an AssetKey.", AeLoggingLevel.Error, model.Key);
                            return;
                        }
                        else throw new Exception($"Asset metadata for asset with key: {model.Key} does not contain an AssetKey.");
                    }

                    var assetCodeForCompilation = GetAssetCodeForCompilation(assetContainer.Metadata.AssetKey, writeLog);

                    if (assetCodeForCompilation != null)
                    {
                        try
                        {
                            //When running in AttachedDebugging mode, the compiled assemblies are expected to be injected.
                            //So we skip compilation.
                            if (Engine.ExecutionMode != AeEngineExecutionMode.AttachedDebugging)
                            {
                                if (!AeRuntimeCompiler.CompileToAssembly(assetContainer.Metadata.AssetKey, assetCodeForCompilation, true, writeLog))
                                    throw new Exception($"Failed to compile asset code for asset with key: {model.Key}. No assembly was returned from the compiler.");
                            }

                            //Save the name of the class that was compiled for this asset so that it can be instantiated later when the asset is requested.
                            // Note that this may also simply be inferred if running in debug mode with "injected" assets.
                            assetContainer.ControllerName = AeRuntimeCompiler.AssetKeyToClassName(assetContainer.Metadata.AssetKey);

                            //Causes the type to be cached in SiReflection for later instantiation when the asset is requested.
                            AeReflection.GetTypeByName(assetContainer.Metadata.DynamicTypeName);
                        }
                        catch (Exception ex)
                        {
                            if (writeLog != null)
                            {
                                writeLog($"Failed to compile asset controller for asset with key: {model.Key}. Error: {ex.Message}", AeLoggingLevel.Error, model.Key);
                            }
                            else throw new Exception($"Failed to compile asset controller for asset with key: {model.Key}. Error: {ex.Message}");
                        }
                    }

                    lock (_collection)
                    {
                        _collection.Add(model.Key, assetContainer);
                    }
                    Interlocked.Increment(ref statusIndex);
                });
            }

            threadPoolTracker.WaitForCompletion(TimeSpan.FromMilliseconds(100), () =>
            {
                progressCallback?.Invoke("Loading assets...", statusIndex / statusEntryCount * 100.0f);
                return true;
            });

            progressCallback?.Invoke("Loading assets...", 100.0f);

            _cache.Clear();

            IsLoaded = true;
        }

        #region Explicit helpers for common assets to avoid typos and ease refactoring.

        /// <summary>
        /// Helper method to get a random gamer tag from the "Text/GamerTags" text asset.
        /// </summary>
        public string GetRandomGamerTag()
        {
            var gamerTagsText = GetText("Text/GamerTags");
            var gamerTags = gamerTagsText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(g => g.Trim()).ToList();

            var randomIndex = AeRandom.Between(0, gamerTags.Count - 1);
            return gamerTags[randomIndex];
        }

        /// <summary>
        /// Helper method to get a random lobby name from the "Text/LobbyNames" text asset.
        /// </summary>
        public string GetRandomLobbyName()
        {
            var gamerTagsText = GetText("Text/LobbyNames");
            var gamerTags = gamerTagsText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(g => g.Trim()).ToList();

            var randomIndex = AeRandom.Between(0, gamerTags.Count - 1);
            return gamerTags[randomIndex];
        }

        #endregion

        internal AssetContainer DeserializeAssetContainer(AssetDatabaseModel model)
        {
            switch (model.BaseType)
            {
                case "json":
                case "cs":
                case "xml":
                case "txt":
                    {
                        var metaData = JsonSerializer.Deserialize<AssetMetadata>(model.Metadata, AeConstants.JsonSerializerOptions)
                           ?? throw new Exception($"Failed to deserialize metadata for asset: {model.Key}");
                        var bytes = model.IsCompressed ? CompressionHelper.Decompress(model.Bytes) : model.Bytes;
                        var obj = Encoding.UTF8.GetString(bytes);

                        return new AssetContainer(model.Key, model.BaseType, metaData, obj);
                    }
                case "png":
                case "bmp":
                    {
                        var metaData = JsonSerializer.Deserialize<AssetMetadata>(model.Metadata, AeConstants.JsonSerializerOptions)
                                  ?? throw new Exception($"Failed to deserialize metadata for asset: {model.Key}");
                        var bytes = model.IsCompressed ? CompressionHelper.Decompress(model.Bytes) : model.Bytes;
                        using var stream = new MemoryStream(bytes);
                        var obj = Engine.Rendering.BitmapStreamToD2DBitmap(stream);

                        return new AssetContainer(model.Key, model.BaseType, metaData, obj);
                    }
                case "wav":
                    {
                        var metaData = JsonSerializer.Deserialize<AssetMetadata>(model.Metadata, AeConstants.JsonSerializerOptions)
                                  ?? throw new Exception($"Failed to deserialize metadata for asset: {model.Key}");
                        var bytes = model.IsCompressed ? CompressionHelper.Decompress(model.Bytes) : model.Bytes;
                        using var stream = new MemoryStream(bytes);
                        var obj = new AeAudioClip(stream, metaData.SoundVolume ?? 1, metaData.LoopSound ?? false);

                        return new AssetContainer(model.Key, model.BaseType, metaData, obj);
                    }
                default:
                    throw new Exception($"Deserialization of the type {model.BaseType} for {model.Key} is not implemented.");
            }
        }

        /// <summary>
        /// Writes an asset to the database. This is really only intended for use in the editor.
        /// It will overwrite any existing asset with the same key and refreshes the asset in the collection.
        /// </summary>
        public void WriteEmptyAsset(string assetKey, string baseType)
        {
            _cache.Clear();

            _assetsDatabase.Execute("DELETE FROM Assets WHERE Key = @Key", new { Key = assetKey });

            var metadata = new AssetMetadata()
            {
                AssetKey = assetKey
            };

            _assetsDatabase.Execute("INSERT INTO Assets (Key, BaseType, Bytes, IsCompressed, Metadata)"
                + "VALUES (@Key, @BaseType, @Bytes, @IsCompressed, @Metadata)",
                new
                {
                    Key = assetKey,
                    Bytes = Array.Empty<byte>(),
                    Metadata = JsonSerializer.Serialize(metadata, AeConstants.JsonSerializerOptions),
                    IsCompressed = false,
                    BaseType = baseType.ToLower()
                });

            RefreshAssetIntoCollection(assetKey);
        }

        private byte[] CompressAsset(byte[] bytes, string baseType)
        {
            if (baseType == "cs" || baseType == "txt" || baseType == "json" || baseType == "xml")
            {
                //Just for the sake of easy database editing, we do not compress text based assets.
                return bytes;
            }
            return CompressionHelper.Compress(bytes, CompressionLevel.SmallestSize);
        }

        /// <summary>
        /// Writes an asset to the database. This is really only intended for use in the editor.
        /// It will overwrite any existing asset with the same key and refreshes the asset in the collection.
        /// </summary>
        public void WriteAssetFromFile(string assetKey, string filePath, AssetMetadata metadata)
        {
            _cache.Clear();

            var baseType = Path.GetExtension(filePath).Trim('.').ToLower();

            var originalFileBytes = File.ReadAllBytes(filePath);
            var compressedBytes = CompressAsset(originalFileBytes, baseType);
            _assetsDatabase.Execute("DELETE FROM Assets WHERE Key = @Key", new { Key = assetKey });

            metadata.AssetKey = assetKey;

            _assetsDatabase.Execute("INSERT INTO Assets (Key, BaseType, Bytes, IsCompressed, Metadata)"
                + "VALUES (@Key, @BaseType, @Bytes, @IsCompressed, @Metadata)",
                new
                {
                    Key = assetKey,
                    Bytes = originalFileBytes.Length > compressedBytes.Length ? compressedBytes : originalFileBytes,
                    IsCompressed = originalFileBytes.Length > compressedBytes.Length,
                    Metadata = JsonSerializer.Serialize(metadata, AeConstants.JsonSerializerOptions),
                    BaseType = baseType
                });

            RefreshAssetIntoCollection(assetKey);
        }

        /// <summary>
        /// Writes an assets metadata to the database and refreshes the asset in the collection.
        /// </summary>
        /// <param name="assetKey"></param>
        /// <param name="metadata"></param>
        public void WriteAssetMetadata(string assetKey, AssetMetadata metadata)
        {
            _cache.Clear();

            metadata.AssetKey = assetKey;

            _assetsDatabase.Execute("UPDATE Assets SET Metadata = @Metadata WHERE Key = @Key",
                new
                {
                    Key = assetKey,
                    Metadata = JsonSerializer.Serialize(metadata, AeConstants.JsonSerializerOptions)
                });

            RefreshAssetIntoCollection(assetKey);
        }

        /// <summary>
        /// Writes an assets bytes (such as an image, wav file, text, etc.) to the database and refreshes the asset in the collection.
        /// This is really only intended for use in the editor.
        /// </summary>
        /// <param name="assetKey"></param>
        /// <param name="filePath"></param>
        public void WriteAssetBytesFromFile(string assetKey, string filePath)
        {
            _cache.Clear();

            var originalFileBytes = File.ReadAllBytes(filePath);
            var baseType = Path.GetExtension(filePath).Trim('.').ToLower();
            var compressedBytes = CompressAsset(originalFileBytes, baseType);

            _assetsDatabase.Execute("UPDATE Assets SET BaseType = @BaseType, Bytes = @Bytes, IsCompressed = @IsCompressed WHERE Key = @Key",
                new
                {
                    Key = assetKey,
                    Bytes = originalFileBytes.Length > compressedBytes.Length ? compressedBytes : originalFileBytes,
                    IsCompressed = originalFileBytes.Length > compressedBytes.Length,
                    BaseType = baseType
                });

            RefreshAssetIntoCollection(assetKey);
        }

        /// <summary>
        /// Writes an assets bytes for a text asset such as (text, code, json, xml, etc.) to the database and refreshes the asset in the collection.
        /// </summary>
        /// <param name="assetKey"></param>
        /// <param name="controllerText"></param>
        public void WriteAssetControllerFromText(string assetKey, string controllerText)
        {
            _cache.Clear();

            _assetsDatabase.Execute("UPDATE Assets SET Controller = @Controller, IsCompressed = @IsCompressed WHERE Key = @Key",
                new
                {
                    Key = assetKey,
                    Controller = controllerText,
                    IsCompressed = false
                });

            RefreshAssetIntoCollection(assetKey);
        }

        /// <summary>
        /// Writes an assets bytes for a text asset such as (text, code, json, xml, etc.) to the database and refreshes the asset in the collection.
        /// </summary>
        /// <param name="assetKey"></param>
        /// <param name="objectText"></param>
        public void WriteAssetBytesFromText(string assetKey, string objectText)
        {
            _cache.Clear();

            _assetsDatabase.Execute("UPDATE Assets SET Bytes = @Bytes, IsCompressed = @IsCompressed WHERE Key = @Key",
                new
                {
                    Key = assetKey,
                    Bytes = Encoding.UTF8.GetBytes(objectText),
                    IsCompressed = false
                });

            RefreshAssetIntoCollection(assetKey);
        }

        /// <summary>
        ///  Refreshes an asset in the collection from the database.
        /// </summary>
        /// <param name="assetKey"></param>
        public void RefreshAssetIntoCollection(string assetKey)
        {
            var model = _assetsDatabase.QueryFirst<AssetDatabaseModel>("SELECT Key, BaseType, Bytes, IsCompressed, Metadata FROM Assets WHERE Key = @Key",
                new { Key = assetKey });

            var asset = DeserializeAssetContainer(model);
            lock (_collection)
            {
                _collection[model.Key] = asset;
            }

            _cache.Clear();
        }

        /// <summary>
        /// Reads the asset bytes (such as an image, wav file, text, etc.) from the database and returns them.
        /// </summary>
        public byte[] ReadAssetBytes(string assetKey)
        {
            var model = _assetsDatabase.QueryFirst<AssetDatabaseModel>("SELECT Key, BaseType, Bytes, Metadata FROM Assets WHERE Key = @Key",
                new { Key = assetKey });

            return model.IsCompressed ? CompressionHelper.Decompress(model.Bytes) : model.Bytes;
        }

        /// <summary>
        /// Used to read the C# controller for the asset because we do not store it in memory in an uncompiled form.
        /// </summary>
        public string ReadAssetController(string assetKey)
        {
            return _assetsDatabase.QueryFirst<string>("SELECT Controller FROM Assets WHERE Key = @Key",
                new { Key = assetKey });
        }

        /// <summary>
        /// Reads the asset bytes (such as an image, wav file, text, etc.) from the database and returns them.
        /// </summary>
        public void DeleteAsset(string assetKey)
        {
            _assetsDatabase.Execute("DELETE FROM Assets WHERE Key = @Key",
                new { Key = assetKey });
        }

        /// <summary>
        /// Extracts the assets to a buildable Visual Studio project format on disk.
        /// This is only intended for use in the editor and is not optimized for performance.
        /// </summary>
        public void ExtractProject(string extractPath, WriteLogDelegate writeLog)
        {
            var archiveBytes = AeEmbeddedResourceReader.LoadBytes("Compiler/Templates/AeDebugProjectTemplate.zip");

            using var ms = new MemoryStream(archiveBytes);
            using var archive = new ZipArchive(ms, ZipArchiveMode.Read);
            var version = string.Join('.', (Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0").Split('.').Take(3));

            var assetsFullPath = Path.GetFullPath(Engine.AssetPackagePath);

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
            var database = new SqliteManagedFactory($"Data Source={Engine.AssetPackagePath}");

            var assets = database.Query<AssetDatabaseModel>(
                "SELECT Key, BaseType, Controller, Bytes, IsCompressed, Metadata FROM Assets").ToList();

            foreach (var asset in assets)
            {
                Console.WriteLine($"[{asset.Key}]");

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
                    var codeToCompile = Engine.Assets.GetAssetCodeForCompilation(asset.Key, writeLog);
                    File.WriteAllText(Path.Combine(extractPath, asset.Key + "." + asset.BaseType), codeToCompile);
                }

                if (!string.IsNullOrWhiteSpace(asset.Metadata))
                    File.WriteAllText(Path.Combine(extractPath, asset.Key + "." + asset.BaseType + ".meta"), asset.Metadata);

                if (!string.IsNullOrWhiteSpace(asset.Controller))
                {
                    var codeToCompile = Engine.Assets.GetAssetCodeForCompilation(asset.Key, writeLog);
                    File.WriteAllText(Path.Combine(extractPath, asset.Key + "." + asset.BaseType + ".code.cs"), codeToCompile);

                }
            }
        }
    }
}
