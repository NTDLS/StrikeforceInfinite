using SharpDX.Mathematics.Interop;
using System.Drawing;

namespace Ae.Engine.ExtensionMethods
{
    /// <summary>
    /// Provides extension methods for converting a System.Drawing.RectangleF to a RawRectangleF structure.
    /// </summary>
    public static class AeRectangleFExtensions
    {
        /// <summary>
        /// Converts a specified RectangleF structure to a RawRectangleF structure with equivalent coordinates.
        /// </summary>
        /// <param name="rectangle">The RectangleF structure to convert. Represents the rectangle to be transformed into RawRectangleF
        /// coordinates.</param>
        /// <returns>A RawRectangleF structure containing the same position and size as the specified RectangleF.</returns>
        public static RawRectangleF ToRawRectangleF(this RectangleF rectangle)
        {
            return new RawRectangleF(
                        rectangle.X, rectangle.Y,
                        rectangle.X + rectangle.Width,
                        rectangle.Y + rectangle.Height);
        }
    }
}
