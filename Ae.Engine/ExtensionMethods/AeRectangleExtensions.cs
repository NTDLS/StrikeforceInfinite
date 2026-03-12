using Ae.Engine.Mathematics;
using System.Drawing;

namespace Ae.Engine.ExtensionMethods
{
    /// <summary>
    /// Provides extension methods for the RectangleF structure to support cloning, inflating, and intersection
    /// operations with additional flexibility.
    /// </summary>
    /// <remarks>These methods enable convenient manipulation of RectangleF instances, such as creating
    /// expanded rectangles or determining intersection with optional tolerance. The extensions are intended to simplify
    /// common geometric operations in graphical applications.</remarks>
    public static class AeRectangleExtensions
    {
        /// <summary>
        /// Clones a float rectangle.
        /// </summary>
        public static RectangleF Clone(this RectangleF rectangle)
        {
            return new RectangleF(rectangle.Location, rectangle.Size);
        }

        /// <summary>
        /// Returns a new rectangle that is inflated by the specified size vector.
        /// </summary>
        /// <remarks>This method creates a copy of the original rectangle and inflates it by the specified
        /// size. The original rectangle remains unchanged.</remarks>
        /// <param name="rectangle">The rectangle to be inflated. The original rectangle is not modified.</param>
        /// <param name="size">The vector specifying the amount to inflate the rectangle along the X and Y axes.</param>
        /// <returns>A new RectangleF instance representing the inflated rectangle.</returns>
        public static RectangleF Balloon(this RectangleF rectangle, AeVector size)
        {
            var rec = rectangle.Clone();
            rec.Inflate(size.X, size.Y);
            return rec;
        }

        /// <summary>
        /// Returns a new rectangle that is inflated by the specified horizontal and vertical amounts.
        /// </summary>
        /// <remarks>This method creates a copy of the original rectangle and inflates it by the specified
        /// amounts. The original rectangle remains unchanged.</remarks>
        /// <param name="rectangle">The rectangle to be inflated. The original rectangle is not modified.</param>
        /// <param name="x">The amount, in pixels, to inflate the rectangle horizontally.</param>
        /// <param name="y">The amount, in pixels, to inflate the rectangle vertically.</param>
        /// <returns>A new RectangleF instance representing the inflated rectangle.</returns>
        public static RectangleF Balloon(this RectangleF rectangle, float x, float y)
        {
            var rec = rectangle.Clone();
            rec.Inflate(x, y);
            return rec;
        }

        /// <summary>
        /// Returns a new rectangle that is inflated by the specified horizontal and vertical amounts.
        /// </summary>
        /// <remarks>The original rectangle is not modified. Positive values for <paramref name="x"/> and
        /// <paramref name="y"/> increase the size of the rectangle; negative values decrease it.</remarks>
        /// <param name="rectangle">The rectangle to be inflated.</param>
        /// <param name="x">The amount, in pixels, to inflate the rectangle horizontally.</param>
        /// <param name="y">The amount, in pixels, to inflate the rectangle vertically.</param>
        /// <returns>A new RectangleF instance that is larger or smaller than the original, depending on the values of <paramref
        /// name="x"/> and <paramref name="y"/>.</returns>
        public static RectangleF Balloon(this RectangleF rectangle, int x, int y)
        {
            var rec = rectangle.Clone();
            rec.Inflate(x, y);
            return rec;
        }

        /// <summary>
        /// Returns a new rectangle that is inflated by the specified amount in both the horizontal and vertical
        /// directions.
        /// </summary>
        /// <remarks>The method creates a copy of the original rectangle and inflates it by the specified
        /// value. Negative values for <paramref name="xy"/> will deflate the rectangle.</remarks>
        /// <param name="rectangle">The rectangle to be inflated. The original rectangle is not modified.</param>
        /// <param name="xy">The amount, in units, by which to inflate the rectangle along both the X and Y axes.</param>
        /// <returns>A new RectangleF instance representing the inflated rectangle.</returns>
        public static RectangleF Balloon(this RectangleF rectangle, float xy)
        {
            var rec = rectangle.Clone();
            rec.Inflate(xy, xy);
            return rec;
        }

        /// <summary>
        /// Returns a new rectangle that is inflated by the specified amount in both the horizontal and vertical
        /// directions.
        /// </summary>
        /// <remarks>The original rectangle is not modified. If the specified amount is zero, the returned
        /// rectangle will be identical to the input.</remarks>
        /// <param name="rectangle">The rectangle to be inflated.</param>
        /// <param name="xy">The amount, in pixels, to inflate the rectangle along both the X and Y axes. Must be non-negative.</param>
        /// <returns>A new RectangleF instance that is larger than the original by the specified amount on all sides.</returns>
        public static RectangleF Balloon(this RectangleF rectangle, int xy)
        {
            var rec = rectangle.Clone();
            rec.Inflate(xy, xy);
            return rec;
        }

        /// <summary>
        /// Determines if the rectangle is inside of another rectangle.
        /// </summary>
        public static bool IntersectsWith(this RectangleF reference, RectangleF with, float tolerance)
        {
            return with.X < reference.X + reference.Width + tolerance
                && reference.X < with.X + with.Width + tolerance
                && with.Y < reference.Y + reference.Height + tolerance
                && reference.Y < with.Y + with.Height + tolerance;
        }
    }
}
