
using NTDLS.DelegateThreadPooling;
using NTDLS.SqliteDapperWrapper;
using Si.Audio;
using Si.Engine.Sprite;
using Si.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Si.Engine.Manager
{
    public class AssetManager
    {
        //public static string AssetPackagePath => _assetPackagePath;
#if DEBUG
        public const string AssetPackagePath = "../../../../Installer/Si.Assets.db";
#else
        public const string AssetPackagePath = "Si.Assets.db";
#endif
        public bool IsLoaded { get; private set; }
        private readonly EngineCore _engine;
        private readonly Dictionary<string, AssetContainer> _collection = new();
        private readonly SqliteManagedFactory _assetsDatabase = new($"Data Source={AssetPackagePath}");
        private readonly SiCache _cache = new(SiCache.CacheExpirationScheme.Sliding, TimeSpan.FromSeconds(600));

        public AssetManager(EngineCore engine)
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

        public SiAudioClip GetAudio(string assetKey, float? volume = null)
        {
            if (_collection.TryGetValue(assetKey, out AssetContainer? assetContainer))
            {
                var audioClip = assetContainer.Object as SiAudioClip
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

        public void LoadAllAssets(SpriteTextBlock? loadingHeader, SpriteTextBlock? loadingDetail)
        {
            loadingHeader?.SetTextAndCenterX("Loading packed assets...");

            using var dtp = new DelegateThreadPool(new DelegateThreadPoolConfiguration()
            {
                InitialThreadCount = Environment.ProcessorCount * 4,
                MaximumThreadCount = Environment.ProcessorCount * 4,
            });
            var threadPoolTracker = dtp.CreateChildPool();

            var models = _assetsDatabase.Query<AssetDatabaseModel>("SELECT Key, BaseType, Bytes, IsCompressed, Metadata FROM Assets");

            int statusIndex = 0;
            float statusEntryCount = models.Count();

            foreach (var model in models)
            {
                threadPoolTracker.Enqueue(() =>
                {
                    var asset = DeserializeAssetContainer(model);
                    lock (_collection)
                    {
                        _collection.Add(model.Key, asset);
                    }
                    Interlocked.Increment(ref statusIndex);
                });
            }

            threadPoolTracker.WaitForCompletion(TimeSpan.FromMilliseconds(100), () =>
            {
                loadingDetail?.SetTextAndCenterX($"{statusIndex / statusEntryCount * 100.0:n0}%");
                return true;
            });

            loadingDetail?.SetTextAndCenterX($"100%");

            _cache.Clear();

            IsLoaded = true;
        }

        #region Explicit helpers for common assets to avoid typos and ease refactoring.

        public string GetRandomGamerTag()
        {
            var gamerTagsText = GetText("Text/GamerTags");
            var gamerTags = gamerTagsText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(g => g.Trim()).ToList();

            var randomIndex = SiRandom.Between(0, gamerTags.Count - 1);
            return gamerTags[randomIndex];
        }

        public string GetRandomLobbyName()
        {
            var gamerTagsText = GetText("Text/LobbyNames");
            var gamerTags = gamerTagsText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(g => g.Trim()).ToList();

            var randomIndex = SiRandom.Between(0, gamerTags.Count - 1);
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
                        var metaData = JsonSerializer.Deserialize<AssetMetadata>(model.Metadata, SiConstants.JsonSerializerOptions)
                           ?? throw new Exception($"Failed to deserialize metadata for asset: {model.Key}");
                        var bytes = model.IsCompressed ? CompressionHelper.Decompress(model.Bytes) : model.Bytes;
                        var obj = Encoding.UTF8.GetString(bytes);

                        return new AssetContainer(model.Key, model.BaseType, metaData, obj);
                    }
                case "png":
                case "jpg":
                case "bmp":
                    {
                        var metaData = JsonSerializer.Deserialize<AssetMetadata>(model.Metadata, SiConstants.JsonSerializerOptions)
                                  ?? throw new Exception($"Failed to deserialize metadata for asset: {model.Key}");
                        var bytes = model.IsCompressed ? CompressionHelper.Decompress(model.Bytes) : model.Bytes;
                        using var stream = new MemoryStream(bytes);
                        var obj = _engine.Rendering.BitmapStreamToD2DBitmap(stream);

                        return new AssetContainer(model.Key, model.BaseType, metaData, obj);
                    }
                case "wav":
                    {
                        var metaData = JsonSerializer.Deserialize<AssetMetadata>(model.Metadata, SiConstants.JsonSerializerOptions)
                                  ?? throw new Exception($"Failed to deserialize metadata for asset: {model.Key}");
                        var bytes = model.IsCompressed ? CompressionHelper.Decompress(model.Bytes) : model.Bytes;
                        using var stream = new MemoryStream(bytes);
                        var obj = new SiAudioClip(stream, metaData.SoundVolume ?? 1, metaData.LoopSound ?? false);

                        return new AssetContainer(model.Key, model.BaseType, metaData, obj);
                    }
                default:
                    throw new Exception($"Deserialization of the type {model.BaseType} for {model.Key} is not implemented.");
            }
        }

        /// <summary>
        /// Writes an asset to the database. This is really only intended for use in the editor, but it can be used at runtime if needed.
        /// It will overwrite any existing asset with the same key.
        /// </summary>
        public void WriteAsset(string assetKey, string filePath, AssetMetadata metadata)
        {
            _cache.Clear();
            long originalSize = new FileInfo(filePath).Length;

            var originalFileBytes = File.ReadAllBytes(filePath);
            var compressedBytes = CompressionHelper.Compress(originalFileBytes, CompressionLevel.SmallestSize);
            long compressedSize = compressedBytes.Length;

            double ratio = originalSize == 0
                ? 0
                : 100.0 * (originalSize - compressedSize) / originalSize;

            _assetsDatabase.Execute("DELETE FROM Assets WHERE Key = @Key", new { Key = assetKey });

            _assetsDatabase.Execute("INSERT INTO Assets (Key, BaseType, Bytes, IsCompressed, Metadata)"
                + "VALUES (@Key, @BaseType, @Bytes, @IsCompressed, @Metadata)",
                new
                {
                    Key = assetKey,
                    Bytes = ratio >= SiConstants.MinimumCompressionRatio ? compressedBytes : originalFileBytes,
                    IsCompressed = ratio >= SiConstants.MinimumCompressionRatio ? true : false,
                    Metadata = JsonSerializer.Serialize(metadata, SiConstants.JsonSerializerOptions),
                    BaseType = Path.GetExtension(filePath).Trim('.').ToLower()
                });
        }
    }
}
