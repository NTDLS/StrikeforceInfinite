using Newtonsoft.Json;
using NTDLS.DelegateThreadPooling;
using NTDLS.Helpers;
using NTDLS.Semaphore;
using SharpCompress.Archives;
using SharpCompress.Common;
using Si.Audio;
using Si.Engine.Sprite;
using Si.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Si.Engine.Manager
{
    public class AssetManager : IDisposable
    {
#if DEBUG
        private const string _assetPackagePath = "../../../../Installer/Si.Assets.rez";
#else
        private const string _assetPackagePath = "Si.Assets.rez";
#endif

        public enum BaseAssetType
        {
            Meta,
            Asset
        }

        public class MetadataContainer
        {
            public AssetContainer Asset { get; set; }
            public Metadata Metadata { get; set; }

            public MetadataContainer(AssetContainer container, Metadata metadata)
            {
                Asset = container;
                Metadata = metadata;
            }
        }

        public class AssetContainer
        {
            public BaseAssetType BaseAssetType { get; set; }
            public string? Directory { get; set; }
            public string SpritePath { get; set; }
            public object Object { get; set; }

            public AssetContainer(BaseAssetType baseAssetType, string spritePath, object obj)
            {
                BaseAssetType = baseAssetType;
                Directory = Path.GetDirectoryName(spritePath)?.ToLower();
                SpritePath = spritePath;
                Object = obj;
            }
        }

        public string AssetPackagePath => _assetPackagePath;
        private readonly EngineCore _engine;
        private readonly OptimisticCriticalResource<Dictionary<string, AssetContainer>> _collection = new();
        private readonly IArchive? _archive = null;
        private readonly Dictionary<string, IArchiveEntry> _entryHashes;

        public Dictionary<string, IArchiveEntry> Entries => _entryHashes;

        public AssetManager(EngineCore engine)
        {
            _engine = engine;

            _archive = ArchiveFactory.Open(_assetPackagePath, new SharpCompress.Readers.ReaderOptions()
            {
                ArchiveEncoding = new ArchiveEncoding()
                {
                    Default = System.Text.Encoding.Default
                }
            });

            _entryHashes = _archive.Entries.ToDictionary(item => item.Key.EnsureNotNull().ToLower(), item => item);
        }

        public void Dispose()
        {
            _archive?.Dispose();
        }

        public static bool IsDirectoryFromAttrib(IEntry entry) =>
            entry.Attrib.HasValue && ((FileAttributes)entry.Attrib.Value & FileAttributes.Directory) != 0;

        /// <summary>
        /// Gets the metadata for all assets in a directory.
        /// This REQUIRES that the assets already be cached.
        /// </summary>
        public List<MetadataContainer> GetMetadataInDirectory(string directory, bool avoidCache = false)
        {
            var assetMetadatas = _collection.Read(o =>
                o.Where(kv => kv.Value.BaseAssetType == BaseAssetType.Meta
                && string.Equals(kv.Value.Directory, directory, StringComparison.OrdinalIgnoreCase))
                .Select(kv => new MetadataContainer(kv.Value, (Metadata)kv.Value.Object))).ToList();

            return assetMetadatas ?? [];
        }

        public Metadata GetMetadata(string spritePath, bool avoidCache = false)
        {
            string metadataFile = $"{spritePath}.meta".Replace('\\', '/');

            if (avoidCache)
            {
                return JsonConvert.DeserializeObject<Metadata>(GetText(metadataFile)) ?? new Metadata();
            }

            string key = $"meta:{metadataFile.ToLower()}";

            var cached = _collection.Read(o =>
            {
                o.TryGetValue(key, out var value);
                return value?.Object;
            });

            if (cached != null)
            {
                return (Metadata)cached;
            }

            var metadata = JsonConvert.DeserializeObject<Metadata>(GetText(metadataFile)) ?? new Metadata();
            _collection.Write(o => o.Add(key, new AssetContainer(BaseAssetType.Meta, spritePath, metadata.EnsureNotNull())));
            return metadata;
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

        public string GetText(string path, string defaultText = "")
        {
            path = path.ToLower().Replace('\\', '/');

            var cached = _collection.Read(o =>
            {
                o.TryGetValue(path, out var value);
                return value?.Object as string;
            });
            if (cached != null)
            {
                return cached;
            }

            try
            {
                var text = GetCompressedText(path);
                _collection.Write(o => o.TryAdd(path, new AssetContainer(BaseAssetType.Asset, path, text)));
                return text;
            }
            catch
            {
                return defaultText;
            }
        }

        public SiAudioClip GetAudio(string path, float initialVolume = 1, bool loopForever = false)
        {
            path = path.ToLower().Replace('\\', '/');

            var cacheKey = $"{path}:{initialVolume}:{loopForever}";
            var cached = _collection.Read(o =>
            {
                if (o.TryGetValue(cacheKey, out var value))
                {
                    var audioClip = value.Object as SiAudioClip;
                    audioClip?.SetInitialVolume(initialVolume);
                    audioClip?.SetLoopForever(loopForever);
                    return audioClip;
                }
                return null;
            });

            if (cached != null)
            {
                return cached;
            }

            using var stream = GetCompressedStream(path);
            var result = new SiAudioClip(stream, initialVolume, loopForever);
            _collection.Write(o => o.TryAdd(cacheKey, new AssetContainer(BaseAssetType.Asset, path, result)));
            stream.Close();
            return result;
        }

        public SharpDX.Direct2D1.Bitmap GetBitmap(string path)
        {
            path = path.ToLower().Replace('\\', '/');

            var cached = _collection.Read(o =>
            {
                o.TryGetValue(path, out var value);
                return value?.Object as SharpDX.Direct2D1.Bitmap;
            });

            if (cached != null)
            {
                return cached;
            }

            using var stream = GetCompressedStream(path);
            var bitmap = _engine.Rendering.BitmapStreamToD2DBitmap(stream);
            _collection.Write(o => o.TryAdd(path, new AssetContainer(BaseAssetType.Asset, path, bitmap)));
            return bitmap;
        }

        public void HydrateCache(SpriteTextBlock? loadingHeader, SpriteTextBlock? loadingDetail)
        {
            loadingHeader?.SetTextAndCenterX("Loading packed assets...");

            using var archive = ArchiveFactory.Open(_assetPackagePath);
            using var dtp = new DelegateThreadPool(new DelegateThreadPoolConfiguration()
            {
                InitialThreadCount = Environment.ProcessorCount
            });
            var threadPoolTracker = dtp.CreateChildPool();

            int statusIndex = 0;
            float statusEntryCount = archive.Entries.Count();

            foreach (var entry in archive.Entries)
            {
                /// Skip entries with '@' in the name as they are likely to be used for internal purposes and not actual assets.
                if (entry.Key?.Contains('@') == true) continue;

                switch (Path.GetExtension(entry.Key.EnsureNotNull()).ToLower())
                {
                    case ".meta":
                    case ".json":
                    case ".txt":
                        threadPoolTracker.Enqueue(() => GetText(entry.Key), (QueueItemState<object> o) => Interlocked.Increment(ref statusIndex));
                        break;
                    case ".png":
                    case ".jpg":
                    case ".bmp":
                        threadPoolTracker.Enqueue(() => GetBitmap(entry.Key), (QueueItemState<object> o) => Interlocked.Increment(ref statusIndex));
                        break;
                    case ".wav":
                        threadPoolTracker.Enqueue(() => GetAudio(entry.Key), (QueueItemState<object> o) => Interlocked.Increment(ref statusIndex));
                        break;
                    default:
                        Interlocked.Increment(ref statusIndex);
                        break;
                }

                if (!IsDirectoryFromAttrib(entry) && !entry.Key.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                {
                    GetMetadata(entry.Key);
                }
            }

            threadPoolTracker.WaitForCompletion(TimeSpan.FromMilliseconds(100), () =>
            {
                loadingDetail?.SetTextAndCenterX($"{statusIndex / statusEntryCount * 100.0:n0}%");
                return true;
            });

            loadingDetail?.SetTextAndCenterX($"100%");
        }

        private string GetCompressedText(string path)
        {
            using var stream = GetCompressedStream(path);
            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }

        private MemoryStream GetCompressedStream(string path)
        {
            path = path.Trim().Replace("\\", "/");

            if (_entryHashes.TryGetValue(path, out var entry))
            {
                lock (_entryHashes)
                {
                    using var stream = entry.OpenEntryStream();
                    var memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    return memoryStream;
                }
            }

            throw new FileNotFoundException(path);
        }

        #region Explicit helpers for common assets to avoid typos and ease refactoring.

        public string GetRandomGamerTag()
        {
            var gamerTagsText = GetText($@"Text\GamerTags.txt");
            var gamerTags = gamerTagsText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(g => g.Trim()).ToList();

            var randomIndex = SiRandom.Between(0, gamerTags.Count - 1);
            return gamerTags[randomIndex];
        }

        public string GetRandomLobbyName()
        {
            var gamerTagsText = GetText($@"Text\LobbyNames.txt");
            var gamerTags = gamerTagsText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(g => g.Trim()).ToList();

            var randomIndex = SiRandom.Between(0, gamerTags.Count - 1);
            return gamerTags[randomIndex];
        }

        #endregion
    }
}
