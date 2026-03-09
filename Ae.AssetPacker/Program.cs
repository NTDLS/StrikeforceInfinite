namespace Ae.AssetPacker
{
    /// <summary>
    /// Used to pack a directory of assets into the database file. This really shouldn't be used anymore.
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
            var sqliteDb = new SqliteManagedFactory("Data Source=../../../../Installer/Ae.Assets.db");

            //Files and paths that contain "@" are ignored because they effectively "Commented out" assets.
            //Files and paths that contain "#" are "internal" assets that we pack but do not show to the user in the editor.

            var assetRoot = @"C:\NTDLS\AxisEngine\Assets";
            var assetPaths = Directory.GetFiles(assetRoot, "*.*", SearchOption.AllDirectories)
                .Where(o => o.Contains("@") == false && Path.GetExtension(o) != ".meta").ToList();

            sqliteDb.Execute("DELETE FROM Assets");

            foreach (var fullAssetPath in assetPaths)
            {
                var directory = Path.GetDirectoryName(fullAssetPath).EnsureNotNull();
                var fileName = Path.GetFileNameWithoutExtension(fullAssetPath);
                var relativePath = Path.GetRelativePath(assetRoot, directory);

                var assetKey = $"{relativePath}\\{fileName}".Replace("\\", "/").Replace("//", "/");

                var originalFileBytes = File.ReadAllBytes(fullAssetPath);
                var compressedBytes = CompressionHelper.Compress(originalFileBytes, CompressionLevel.SmallestSize);

                var metadataJson = File.ReadAllText($"{fullAssetPath}.meta");

                sqliteDb.Execute("DELETE FROM Assets WHERE Key = @Key", new { Key = assetKey });

                var metadata = JsonSerializer.Deserialize<AssetMetadata>(string.IsNullOrWhiteSpace(metadataJson) ? "{}" : metadataJson, AeConstants.JsonSerializerOptions);
                if (metadata != null)
                {
                    metadata.AssetKey = assetKey;

                    sqliteDb.Execute("INSERT INTO Assets (Key, BaseType, Bytes, Metadata)"
                        + "VALUES (@Key, @BaseType, @Bytes, @Metadata)",
                        new
                        {
                            Key = assetKey,
                    Bytes = originalFileBytes.Length > compressedBytes.Length ? compressedBytes : originalFileBytes,
                    IsCompressed = originalFileBytes.Length > compressedBytes.Length,
                            Metadata = JsonSerializer.Serialize(metadata, AeConstants.JsonSerializerOptions),
                            BaseType = Path.GetExtension(fullAssetPath).Trim('.').ToLower()
                        });

                    Console.WriteLine($"[{assetKey}]");
                }
            }
            */
        }
    }
}
