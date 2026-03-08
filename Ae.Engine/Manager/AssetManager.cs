
using Ae.Audio;
using Ae.Library;
using Ae.Library.Compiler;
using Ae.Library.Metadata;
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

        public AeAudioClip GetAudio(string assetKey, float? volume = null)
        {
            if (_collection.TryGetValue(assetKey, out AssetContainer? assetContainer))
            {
                var audioClip = assetContainer.Object as AeAudioClip
                    ?? throw new FileNotFoundException($"Asset could not be converted to audio: {assetKey}");
                audioClip.SetInitialVolume(volume ?? assetContainer.Metadata.SoundVolume ?? 1);
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

        public void LoadAllAssets(Action<string, float>? progressCallback)
        {
            progressCallback?.Invoke("Loading assets...", 0);

            using var dtp = new DelegateThreadPool(new DelegateThreadPoolConfiguration()
            {
                InitialThreadCount = Environment.ProcessorCount * 4,
                MaximumThreadCount = Environment.ProcessorCount * 4,
            });
            var threadPoolTracker = dtp.CreateChildPool();

            var models = _assetsDatabase.Query<AssetDatabaseModel>("SELECT Key, BaseType, Controller, Bytes, Metadata FROM Assets");

            int statusIndex = 0;
            float statusEntryCount = models.Count();

            foreach (var model in models)
            {
                threadPoolTracker.Enqueue(() =>
                {
                    var assetContainer = DeserializeAssetContainer(model);

                    if (!string.IsNullOrWhiteSpace(model.Controller)
                        && !string.IsNullOrWhiteSpace(assetContainer.Metadata.Class)
                        && !string.IsNullOrWhiteSpace(assetContainer.Metadata.AssetKey))
                    {
                        var assetClassName = assetContainer.Metadata.AssetKey.Replace('/', '_').Replace('.', '_').Replace(' ', '_');

                        var classCode = AeAssetControllerClassText.Get(assetContainer.Metadata.Class, assetClassName, model.Controller);

                        AeRuntimeCompiler.CompileToAssembly(classCode);

                        //Causes the type to be cached in SiReflection for later instantiation when the asset is requested.
                        AeReflection.GetTypeByName(assetClassName);

                        assetContainer.ControllerName = assetClassName;
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
                case "txt":
                    {
                        var metaData = JsonSerializer.Deserialize<AssetMetadata>(model.Metadata, AeConstants.JsonSerializerOptions)
                           ?? throw new Exception($"Failed to deserialize metadata for asset: {model.Key}");
                        var obj = Encoding.UTF8.GetString(model.Bytes);

                        return new AssetContainer(model.Key, model.BaseType, metaData, obj);
                    }
                case "png":
                case "jpg":
                case "bmp":
                    {
                        var metaData = JsonSerializer.Deserialize<AssetMetadata>(model.Metadata, AeConstants.JsonSerializerOptions)
                                  ?? throw new Exception($"Failed to deserialize metadata for asset: {model.Key}");
                        using var stream = new MemoryStream(model.Bytes);
                        var obj = _engine.Rendering.BitmapStreamToD2DBitmap(stream);

                        return new AssetContainer(model.Key, model.BaseType, metaData, obj);
                    }
                case "wav":
                    {
                        var metaData = JsonSerializer.Deserialize<AssetMetadata>(model.Metadata, AeConstants.JsonSerializerOptions)
                                  ?? throw new Exception($"Failed to deserialize metadata for asset: {model.Key}");
                        using var stream = new MemoryStream(model.Bytes);
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

            _assetsDatabase.Execute("INSERT INTO Assets (Key, BaseType, Bytes, Metadata)"
                + "VALUES (@Key, @BaseType, @Bytes, @Metadata)",
                new
                {
                    Key = assetKey,
                    Bytes = Array.Empty<byte>(),
                    Metadata = JsonSerializer.Serialize(metadata, AeConstants.JsonSerializerOptions),
                    BaseType = baseType.ToLower()
                });

            RefreshAssetIntoCollection(assetKey);
        }

        /// <summary>
        /// Writes an asset to the database. This is really only intended for use in the editor.
        /// It will overwrite any existing asset with the same key and refreshes the asset in the collection.
        /// </summary>
        public void WriteAssetFromFile(string assetKey, string filePath, AssetMetadata metadata)
        {
            _cache.Clear();

            var originalFileBytes = File.ReadAllBytes(filePath);
            var compressedBytes = CompressionHelper.Compress(originalFileBytes, CompressionLevel.SmallestSize);

            _assetsDatabase.Execute("DELETE FROM Assets WHERE Key = @Key", new { Key = assetKey });

            metadata.AssetKey = assetKey;

            _assetsDatabase.Execute("INSERT INTO Assets (Key, BaseType, Bytes, Metadata)"
                + "VALUES (@Key, @BaseType, @Bytes, @Metadata)",
                new
                {
                    Key = assetKey,
                    Bytes = originalFileBytes,
                    Metadata = JsonSerializer.Serialize(metadata, AeConstants.JsonSerializerOptions),
                    BaseType = Path.GetExtension(filePath).Trim('.').ToLower()
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
        public void WriteAssetBytes(string assetKey, string filePath)
        {
            _cache.Clear();

            var originalFileBytes = File.ReadAllBytes(filePath);

            _assetsDatabase.Execute("UPDATE Assets SET BaseType = @BaseType, Bytes = @Bytes WHERE Key = @Key",
                new
                {
                    Key = assetKey,
                    Bytes = originalFileBytes,
                    BaseType = Path.GetExtension(filePath).Trim('.').ToLower()
                });

            RefreshAssetIntoCollection(assetKey);
        }

        /// <summary>
        ///  Refreshes an asset in the collection from the database.
        /// </summary>
        /// <param name="assetKey"></param>
        public void RefreshAssetIntoCollection(string assetKey)
        {
            var model = _assetsDatabase.QueryFirst<AssetDatabaseModel>("SELECT Key, BaseType, Bytes, Metadata FROM Assets WHERE Key = @Key",
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
            return _assetsDatabase.QueryFirst<byte[]>("SELECT Bytes FROM Assets WHERE Key = @Key",
                new { Key = assetKey });
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
            var model = _assetsDatabase.QueryFirst<AssetDatabaseModel>("DELETE FROM Assets WHERE Key = @Key",
                new { Key = assetKey });
        }
    }
}
