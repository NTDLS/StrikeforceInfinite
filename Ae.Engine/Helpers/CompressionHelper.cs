using System;
using System.IO;
using System.IO.Compression;

namespace Ae.Engine.Helpers
{
    /// <summary>
    /// Provides static methods for compressing and decompressing byte arrays and files using the Deflate algorithm.
    /// </summary>
    /// <remarks>This class is intended for scenarios where efficient, lossless compression and decompression
    /// of binary data or files is required. All methods are thread-safe and do not maintain any internal state. The
    /// Deflate algorithm is suitable for general-purpose compression, but may not achieve optimal results for all data
    /// types. Use the appropriate compression level to balance speed and compression ratio as needed.</remarks>
    public static class CompressionHelper
    {
        /// <summary>
        /// Compresses the specified byte array using the Deflate algorithm at the given compression level.
        /// </summary>
        /// <remarks>The returned compressed data can be decompressed using a compatible DeflateStream.
        /// This method does not include any additional headers or metadata in the output.</remarks>
        /// <param name="data">The byte array containing the data to compress. If null or empty, the method returns an empty array.</param>
        /// <param name="level">The compression level to apply. Determines the balance between compression speed and size. Defaults to
        /// CompressionLevel.Optimal.</param>
        /// <returns>A byte array containing the compressed data. Returns an empty array if the input data is null or empty.</returns>
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

        /// <summary>
        /// Decompresses a byte array that was compressed using the Deflate algorithm.
        /// </summary>
        /// <remarks>Use this method to restore data previously compressed with Deflate. The caller is
        /// responsible for ensuring the input is in the correct format; invalid or corrupted data may result in
        /// decompression errors.</remarks>
        /// <param name="compressedData">The compressed data to decompress. Must be a valid Deflate-compressed byte array. If null or empty, the
        /// method returns an empty array.</param>
        /// <returns>A byte array containing the decompressed data. Returns an empty array if the input is null or empty.</returns>
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
    }
}

