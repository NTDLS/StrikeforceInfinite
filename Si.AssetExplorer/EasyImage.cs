using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace Si.AssetExplorer
{
    internal class EasyImage
    {
        /// <summary>
        /// Loads any ImageSharp-supported image from disk and returns a PNG byte array (RGBA).
        /// </summary>
        public static byte[] LoadAnyToPngBytes(string filePath, PngCompressionLevel compression = PngCompressionLevel.DefaultCompression)
        {
            // Force decode into a consistent pixel format for your engine.
            using Image<Rgba32> image = Image.Load<Rgba32>(filePath);

            var encoder = new PngEncoder
            {
                CompressionLevel = compression,
                ColorType = PngColorType.RgbWithAlpha
            };

            using var ms = new MemoryStream();
            image.Save(ms, encoder);
            return ms.ToArray();
        }
    }
}

