using System.IO.Compression;

namespace Si.Library
{
    public static class CompressionHelper
    {
        public static byte[] Compress(byte[] data, CompressionLevel level = CompressionLevel.Optimal)
        {
            if (data == null || data.Length == 0)
                return Array.Empty<byte>();

            using var output = new MemoryStream();

            using (var deflate = new DeflateStream(output, level, leaveOpen: true))
            {
                deflate.Write(data, 0, data.Length);
            }

            return output.ToArray();
        }

        public static byte[] Decompress(byte[] compressedData)
        {
            if (compressedData == null || compressedData.Length == 0)
                return Array.Empty<byte>();

            using var input = new MemoryStream(compressedData);
            using var deflate = new DeflateStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();

            deflate.CopyTo(output);

            return output.ToArray();
        }

        public static byte[] CompressFile(string filePath, CompressionLevel level = CompressionLevel.Optimal)
        {
            using var input = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            using var output = new MemoryStream();

            using (var deflate = new DeflateStream(output, level, leaveOpen: true))
            {
                input.CopyTo(deflate);
            }

            return output.ToArray();
        }

        public static void DecompressToFile(byte[] compressedData, string outputPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

            using var input = new MemoryStream(compressedData);
            using var deflate = new DeflateStream(input, CompressionMode.Decompress);
            using var output = new FileStream(
                outputPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None);

            deflate.CopyTo(output);
        }
    }
}

