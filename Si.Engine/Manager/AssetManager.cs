
using NTDLS.DelegateThreadPooling;
using NTDLS.Helpers;
using NTDLS.Semaphore;
using NTDLS.SqliteDapperWrapper;
using Si.Audio;
using Si.Engine.Sprite;
using Si.Library;
using System;
using System.CodeDom;
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
#if DEBUG
        private const string _assetPackagePath = "../../../../Installer/Si.Assets.db";
#else
        private const string _assetPackagePath = "Si.Assets.db";
#endif

        public class AssetContainer
        {
            public string Key { get; set; }
            public SpriteMetadata Metadata { get; set; }
            public object Object { get; set; }
            public string BaseType { get; set; } = string.Empty;

            public AssetContainer(string key, string baseType, SpriteMetadata metadata, object obj)
            {
                Key = key;
                BaseType = baseType;
                Metadata = metadata;
                Object = obj;
            }
        }

        public bool IsLoaded { get; private set; }
        public string AssetPackagePath => _assetPackagePath;
        private readonly EngineCore _engine;
        private readonly OptimisticCriticalResource<Dictionary<string, AssetContainer>> _collection = new();
        private readonly SqliteManagedFactory _assetsDatabase = new($"Data Source={_assetPackagePath}");

        public AssetManager(EngineCore engine)
        {
            _engine = engine;
        }

        /// <summary>
        /// Gets the metadata for all assets in a directory.
        /// This REQUIRES that the assets already be cached.
        /// </summary>
        public List<AssetContainer> GetAssetsInPath(string directory, bool avoidCache = false)
        {
            var assets = _collection.Read(o =>
                o.Where(kv => kv.Key.StartsWith(directory, StringComparison.OrdinalIgnoreCase))
                .Select(kv => kv.Value).ToList()
            );

            return assets;
        }

        public AssetContainer GetAsset(string spritePath, bool avoidCache = false)
        {
            var assetContainer = _collection.Read(o =>
            {
                o.TryGetValue(spritePath, out AssetContainer? value);
                return value;
            }) ?? throw new FileNotFoundException($"Asset not found: {spritePath}");

            return assetContainer;
        }

        public SpriteMetadata GetMetadata(string spritePath, bool avoidCache = false)
        {
            var assetContainer = _collection.Read(o =>
            {
                o.TryGetValue(spritePath, out AssetContainer? value);
                return value;
            }) ?? throw new FileNotFoundException($"Asset not found: {spritePath}");

            return assetContainer.Metadata;
        }

        /// <summary>
        /// Gets and caches a text files content from the asset path.
        /// </summary>
        /// <param name="assetRelativePath"></param>
        /// <param name="defaultText"></param>
        /// <returns></returns>
        public static string GetUserText(string assetRelativePath, string defaultText = "")
        {
            assetRelativePath = assetRelativePath.ToLower();

            var userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Strikeforce Infinite");
            if (Directory.Exists(userDataPath) == false)
            {
                Directory.CreateDirectory(userDataPath);
            }
            string assetAbsolutePath = Path.Combine(userDataPath, assetRelativePath).Trim().Replace("\\", "/");
            if (File.Exists(assetAbsolutePath) == false)
            {
                return defaultText;
            }

            return File.ReadAllText(assetAbsolutePath);
        }

        /// <summary>
        /// Saves and caches a text file into the asset path.
        /// </summary>
        /// <param name="assetRelativePath"></param>
        /// <param name="value"></param>
        public static void PutUserText(string assetRelativePath, string value)
        {
            assetRelativePath = assetRelativePath.ToLower();

            var userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Strikeforce Infinite");
            if (Directory.Exists(userDataPath) == false)
            {
                Directory.CreateDirectory(userDataPath);
            }
            string assetAbsolutePath = Path.Combine(userDataPath, assetRelativePath).Trim().Replace("\\", "/");
            File.WriteAllText(assetAbsolutePath, value);
        }

        public string GetText(string spritePath, string defaultText = "")
        {
            var assetContainer = _collection.Read(o =>
            {
                o.TryGetValue(spritePath, out AssetContainer? value);
                return value;
            }) ?? throw new FileNotFoundException($"Asset not found: {spritePath}");

            return assetContainer.Object as string ?? defaultText;
        }

        public SiAudioClip GetAudio(string spritePath, float? volume = null)
        {
            var assetContainer = _collection.Read(o =>
            {
                o.TryGetValue(spritePath, out AssetContainer? value);
                return value;
            }) ?? throw new FileNotFoundException($"Asset not found: {spritePath}");


            var audioClip = assetContainer.Object as SiAudioClip;
            audioClip?.SetInitialVolume(volume ?? assetContainer.Metadata.SoundVolume ?? 1);
            audioClip?.SetLoopForever(assetContainer.Metadata.LoopSound ?? false);
            return audioClip ?? throw new FileNotFoundException($"Asset not found: {spritePath}");
        }

        public SharpDX.Direct2D1.Bitmap GetBitmap(string spritePath)
        {
            var assetContainer = _collection.Read(o =>
            {
                o.TryGetValue(spritePath, out AssetContainer? value);
                return value;
            }) ?? throw new FileNotFoundException($"Asset not found: {spritePath}");

            return assetContainer.Object as SharpDX.Direct2D1.Bitmap ?? throw new FileNotFoundException($"Asset not found: {spritePath}");
        }

        public void HydrateCache(SpriteTextBlock? loadingHeader, SpriteTextBlock? loadingDetail)
        {
            loadingHeader?.SetTextAndCenterX("Loading packed assets...");

            using var dtp = new DelegateThreadPool(new DelegateThreadPoolConfiguration()
            {
                InitialThreadCount = Environment.ProcessorCount
            });
            var threadPoolTracker = dtp.CreateChildPool();

            var assets = _assetsDatabase.Query<AssetDatabaseModel>("SELECT Key, BaseType, Bytes, IsCompressed, Metadata FROM assets");

            int statusIndex = 0;
            float statusEntryCount = assets.Count();

            foreach (var asset in assets)
            {
                threadPoolTracker.Enqueue(() =>
                {
                    DeserializeAssetContainer(asset);
                    Interlocked.Increment(ref statusIndex);
                });
            }

            threadPoolTracker.WaitForCompletion(TimeSpan.FromMilliseconds(100), () =>
            {
                loadingDetail?.SetTextAndCenterX($"{statusIndex / statusEntryCount * 100.0:n0}%");
                return true;
            });

            loadingDetail?.SetTextAndCenterX($"100%");

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
            return _collection.Write(collection =>
            {
                switch (model.BaseType)
                {
                    case "json":
                    case "txt":
                        {
                            var metaData = JsonSerializer.Deserialize<SpriteMetadata>(model.Metadata)
                               ?? throw new Exception($"Failed to deserialize metadata for asset: {model.Key}");
                            var bytes = model.IsCompressed ? CompressionHelper.Decompress(model.Bytes) : model.Bytes;
                            var obj = Encoding.UTF8.GetString(bytes);

                            var asset = new AssetContainer(model.Key, model.BaseType, metaData, obj);
                            collection.Add(model.Key, asset);
                            return asset;
                        }
                    case "png":
                    case "jpg":
                    case "bmp":
                        {
                            var metaData = JsonSerializer.Deserialize<SpriteMetadata>(model.Metadata)
                                      ?? throw new Exception($"Failed to deserialize metadata for asset: {model.Key}");
                            var bytes = model.IsCompressed ? CompressionHelper.Decompress(model.Bytes) : model.Bytes;
                            using var stream = new MemoryStream(bytes);
                            var obj = _engine.Rendering.BitmapStreamToD2DBitmap(stream);

                            var asset = new AssetContainer(model.Key, model.BaseType, metaData, obj);
                            collection.Add(model.Key, asset);
                            return asset;
                        }
                    case "wav":
                        {
                            var metaData = JsonSerializer.Deserialize<SpriteMetadata>(model.Metadata)
                                      ?? throw new Exception($"Failed to deserialize metadata for asset: {model.Key}");
                            var bytes = model.IsCompressed ? CompressionHelper.Decompress(model.Bytes) : model.Bytes;
                            using var stream = new MemoryStream(bytes);
                            var obj = new SiAudioClip(stream, metaData.SoundVolume ?? 1, metaData.LoopSound ?? false);

                            var asset = new AssetContainer(model.Key, model.BaseType, metaData, obj);
                            collection.Add(model.Key, asset);
                            return asset;
                        }
                    default:
                        throw new Exception($"Deserialization of the type {model.BaseType} for {model.Key} is not implemented.");
                }
            });
        }

        public void WriteAsset(string assetKey, string filePath, SpriteMetadata metadata)
        {
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
