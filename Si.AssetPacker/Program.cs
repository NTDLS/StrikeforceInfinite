using NTDLS.Helpers;
using NTDLS.SqliteDapperWrapper;
using Si.Library;
using System.IO.Compression;
using System.Security.AccessControl;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Si.AssetPacker
{
    internal class Program
    {
        const int _minCompressedRatio = 1;

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
        };

        static void Main(string[] args)
        {
            var sqliteDb = new SqliteManagedFactory("Data Source=../../../../Installer/Si.Assets.db");

            _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

            //Files and paths that contain "@" are ignored because they effectively "Commented out" assets.
            //Files and paths that contain "#" are "internal" assets that we pack but do not show to the user in the editor.

            var assetRoot = @"C:\NTDLS\StrikeforceInfinite\Assets";
            var assetPaths = Directory.GetFiles(assetRoot, "*.*", SearchOption.AllDirectories)
                .Where(o => o.Contains("@") == false && Path.GetExtension(o) != ".meta").ToList();

            long originalTotalSize = 0;
            long compressedTotalSize = 0;

            sqliteDb.Execute("DELETE FROM Assets");

            foreach (var fullAssetPath in assetPaths)
            {
                var directory = Path.GetDirectoryName(fullAssetPath).EnsureNotNull();
                var fileName = Path.GetFileNameWithoutExtension(fullAssetPath);
                var relativePath = Path.GetRelativePath(assetRoot, directory);

                var assetKey = $"{relativePath}\\{fileName}".Replace("\\", "/").Replace("//", "/");
                long originalSize = new FileInfo(fullAssetPath).Length;
                originalTotalSize += originalSize;

                var originalFileBytes = File.ReadAllBytes(fullAssetPath);
                var compressedBytes = CompressionHelper.Compress(originalFileBytes, CompressionLevel.SmallestSize);

                long compressedSize = compressedBytes.Length;
                compressedTotalSize += compressedSize;

                double ratio = originalSize == 0
                    ? 0
                    : 100.0 * (originalSize - compressedSize) / originalSize;

                var metadataJson = File.ReadAllText($"{fullAssetPath}.meta");

                var metadata = JsonSerializer.Deserialize<SpriteMetadata>(string.IsNullOrWhiteSpace(metadataJson) ? "{}" : metadataJson, _jsonSerializerOptions);

                sqliteDb.Execute("DELETE FROM Assets WHERE Key = @Key", new { Key = assetKey });

                sqliteDb.Execute("INSERT INTO Assets (Key, BaseType, Bytes, IsCompressed, Metadata)"
                    + "VALUES (@Key, @BaseType, @Bytes, @IsCompressed, @Metadata)",
                    new
                    {
                        Key = assetKey,
                        Bytes = ratio >= _minCompressedRatio ? compressedBytes : originalFileBytes,
                        IsCompressed = ratio >= _minCompressedRatio ? true : false,
                        Metadata = JsonSerializer.Serialize(metadata, _jsonSerializerOptions),
                        BaseType = Path.GetExtension(fullAssetPath).Trim('.').ToLower()
                    });

                Console.WriteLine($"[{assetKey}], OriginalSz: {originalSize:n0}, CompressedSz: {compressedBytes.Length:n0} ({ratio:n2}%)");
            }

            double totalRatio = originalTotalSize == 0
                ? 0
                : 100.0 * (originalTotalSize - compressedTotalSize) / originalTotalSize;


            Console.WriteLine($"Original Total Sz: {originalTotalSize:n0} CompressedSz: {compressedTotalSize:n0} ({totalRatio:n2}%)");
        }
    }
}
