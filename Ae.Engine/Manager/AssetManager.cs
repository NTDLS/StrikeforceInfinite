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
using System.Text;
using System.Text.Json;
using System.Threading;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Manager
{
    public class AssetManager
    {
        //public static string AssetPackagePath => _assetPackagePath;
#if DEBUG
        public const string AssetPackagePath = "../../../../Installer/Ae.Assets.db";
#else
        public const string AssetPackagePath = "Ae.Assets.db";
#endif
        public bool IsLoaded { get; private set; }
        private readonly AeEngine _engine;
        private readonly Dictionary<string, AssetContainer> _collection = new();
        private readonly SqliteManagedFactory _assetsDatabase = new($"Data Source={AssetPackagePath}");
        private readonly AeCache _cache = new(AeCache.CacheExpirationScheme.Sliding, TimeSpan.FromSeconds(600));

        public AssetManager(AeEngine engine)
        {
            _engine = engine;
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

        public AssetContainer GetAsset(string assetKey)
        {
            if (_collection.TryGetValue(assetKey, out AssetContainer? assetContainer))
            {
                return assetContainer;
            }
            throw new FileNotFoundException($"Asset not found: {assetKey}");
        }

        public AssetMetadata GetMetadata(string assetKey)
        {
            if (_collection.TryGetValue(assetKey, out AssetContainer? assetContainer))
            {
                return assetContainer.Metadata;
            }
            throw new FileNotFoundException($"Asset not found: {assetKey}");
        }

        public string GetText(string assetKey)
        {
            if (_collection.TryGetValue(assetKey, out AssetContainer? assetContainer))
            {
                return assetContainer.Object as string
                    ?? throw new FileNotFoundException($"Asset could not be converted to text: {assetKey}");
            }
            throw new FileNotFoundException($"Asset not found: {assetKey}");
        }

        public AudioClip GetAudio(string assetKey)
        {
            if (_collection.TryGetValue(assetKey, out AssetContainer? assetContainer))
            {
                var audioClip = assetContainer.Object as AudioClip
                    ?? throw new FileNotFoundException($"Asset could not be converted to audio: {assetKey}");
                audioClip.SetInitialVolume(assetContainer.Metadata.SoundVolume ?? 1);
                audioClip.SetLoopForever(assetContainer.Metadata.LoopSound ?? false);
                return audioClip;
            }
            throw new FileNotFoundException($"Asset not found: {assetKey}");
        }

        public SharpDX.Direct2D1.Bitmap GetBitmap(string assetKey)
        {
            if (_collection.TryGetValue(assetKey, out AssetContainer? assetContainer))
            {
                return assetContainer.Object as SharpDX.Direct2D1.Bitmap
                    ?? throw new FileNotFoundException($"Asset could not be converted to bitmap: {assetKey}");
            }
            throw new FileNotFoundException($"Asset not found: {assetKey}");
        }

        public string? GetAssetCodeForCompilation(string assetKey, Action<string, AeLoggingLevel?>? writeOutput = null)
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
                if (writeOutput != null)
                {
                    writeOutput($"Unsupported asset base type: {model.BaseType} for asset with key: {model.Key}", AeLoggingLevel.Error);
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

        public void LoadAllAssets(Action<string, float>? progressCallback, Action<string, AeLoggingLevel?>? writeOutput = null)
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
                        if (writeOutput != null)
                        {
                            writeOutput($"Unsupported asset base type: {model.BaseType} for asset with key: {model.Key}", AeLoggingLevel.Error);
                        }
                        else throw new Exception($"Unsupported asset base type: {model.BaseType} for asset with key: {model.Key}");
                    }

                    //AeAssetCodeClassText
                    var assetContainer = DeserializeAssetContainer(model);
                    if (string.IsNullOrEmpty(assetContainer.Metadata.AssetKey))
                    {
                        if (writeOutput != null)
                        {
                            writeOutput($"Asset metadata for asset with key: {model.Key} does not contain an AssetKey.", AeLoggingLevel.Error);
                            return;
                        }
                        else throw new Exception($"Asset metadata for asset with key: {model.Key} does not contain an AssetKey.");
                    }

                    var assetCodeForCompilation = GetAssetCodeForCompilation(assetContainer.Metadata.AssetKey, writeOutput);

                    if (assetCodeForCompilation != null)
                    {
                        try
                        {
                            AeRuntimeCompiler.CompileToAssembly(assetContainer.Metadata.AssetKey, assetCodeForCompilation, true, writeOutput);

                            //Causes the type to be cached in SiReflection for later instantiation when the asset is requested.
                            AeReflection.GetTypeByName(assetContainer.Metadata.DynamicTypeName);
                        }
                        catch (Exception ex)
                        {
                            if (writeOutput != null)
                            {
                                writeOutput($"Failed to compile asset controller for asset with key: {model.Key}. Error: {ex.Message}", AeLoggingLevel.Error);
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

        public string GetRandomGamerTag()
        {
            var gamerTagsText = GetText("Text/GamerTags");
            var gamerTags = gamerTagsText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(g => g.Trim()).ToList();

            var randomIndex = AeRandom.Between(0, gamerTags.Count - 1);
            return gamerTags[randomIndex];
        }

        public string GetRandomLobbyName()
        {
            var gamerTagsText = GetText("Text/LobbyNames");
            var gamerTags = gamerTagsText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(g => g.Trim()).ToList();

            var randomIndex = AeRandom.Between(0, gamerTags.Count - 1);
            return gamerTags[randomIndex];
        }

        #endregion

        public AssetContainer DeserializeAssetContainer(AssetDatabaseModel model)
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
                        var obj = _engine.Rendering.BitmapStreamToD2DBitmap(stream);

                        return new AssetContainer(model.Key, model.BaseType, metaData, obj);
                    }
                case "wav":
                    {
                        var metaData = JsonSerializer.Deserialize<AssetMetadata>(model.Metadata, AeConstants.JsonSerializerOptions)
                                  ?? throw new Exception($"Failed to deserialize metadata for asset: {model.Key}");
                        var bytes = model.IsCompressed ? CompressionHelper.Decompress(model.Bytes) : model.Bytes;
                        using var stream = new MemoryStream(bytes);
                        var obj = new AudioClip(stream, metaData.SoundVolume ?? 1, metaData.LoopSound ?? false);

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
    }
}
