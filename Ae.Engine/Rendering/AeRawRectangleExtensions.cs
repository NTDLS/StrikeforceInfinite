using Ae.Engine.Mathematics;
using SharpDX.Mathematics.Interop;

namespace Ae.Engine.Rendering
{
    /// <summary>
    /// Provides extension methods for the RawRectangleF structure to support cloning and resizing operations.
    /// </summary>
    /// <remarks>These methods enable convenient manipulation of rectangle dimensions, such as expanding or
    /// contracting the rectangle by specified amounts. The extension methods are intended to simplify common geometric
    /// operations when working with RawRectangleF instances.</remarks>
    public static class AeRawRectangleExtensions
    {
        /// <summary>
        /// Clones a float rectangle.
        /// </summary>
        public static RawRectangleF Clone(this RawRectangleF rectangle)
        {
            return new RawRectangleF(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
        }

        /// <summary>
        /// Expands the specified rectangle by the given size in all directions.
        /// </summary>
        /// <remarks>The returned rectangle's sides are moved outward by the values in the size vector.
        /// This method does not modify the original rectangle.</remarks>
        /// <param name="rectangle">The rectangle to be expanded. Represents the original area before adjustment.</param>
        /// <param name="size">The vector specifying the amount to expand the rectangle horizontally and vertically. The X component
        /// increases the width, and the Y component increases the height.</param>
        /// <returns>A new rectangle that is larger than the original by the specified size in all directions.</returns>
        public static RawRectangleF Balloon(this RawRectangleF rectangle, AeVector size)
        {
            var rec = rectangle.Clone();
            rec.Left -= size.X;
            rec.Top -= size.Y;
            rec.Right += size.X;
            rec.Bottom += size.Y;
            return rec;
        }

        /// <summary>
        /// Expands the specified rectangle outward by the given horizontal and vertical amounts.
        /// </summary>
        /// <remarks>This method returns a new rectangle with increased size. The expansion is applied
        /// equally to all sides, resulting in the rectangle growing outward from its original boundaries.</remarks>
        /// <param name="rectangle">The rectangle to expand. The original rectangle is not modified.</param>
        /// <param name="x">The amount, in pixels, to expand the rectangle horizontally on both sides.</param>
        /// <param name="y">The amount, in pixels, to expand the rectangle vertically on both sides.</param>
        /// <returns>A new rectangle that is expanded by the specified amounts on all sides.</returns>
        public static RawRectangleF Balloon(this RawRectangleF rectangle, float x, float y)
        {
            var rec = rectangle.Clone();
            rec.Left -= x;
            rec.Top -= y;
            rec.Right += x;
            rec.Bottom += y;
            return rec;
        }

        /// <summary>
        /// Expands the specified rectangle by the given horizontal and vertical amounts, creating a 'ballooned'
        /// rectangle.
        /// </summary>
        /// <remarks>This method increases the size of the rectangle by subtracting from the left and top
        /// edges and adding to the right and bottom edges. The returned rectangle will be larger than the original
        /// unless the expansion values are negative.</remarks>
        /// <param name="rectangle">The rectangle to expand. The original rectangle is not modified.</param>
        /// <param name="x">The amount, in pixels, to expand the rectangle horizontally on both sides.</param>
        /// <param name="y">The amount, in pixels, to expand the rectangle vertically on both sides.</param>
        /// <returns>A new rectangle that is expanded by the specified amounts on all sides.</returns>
        public static RawRectangleF Balloon(this RawRectangleF rectangle, int x, int y)
        {
            var rec = rectangle.Clone();
            rec.Left -= x;
            rec.Top -= y;
            rec.Right += x;
            rec.Bottom += y;
            return rec;
        }

        /// <summary>
        /// Expands the specified rectangle by the given amount in all directions.
        /// </summary>
        /// <remarks>The method returns a new instance and does not modify the original rectangle. If
        /// <paramref name="xy"/> is zero, the returned rectangle will be identical to the input.</remarks>
        /// <param name="rectangle">The rectangle to be expanded.</param>
        /// <param name="xy">The amount, in pixels, to expand the rectangle on each side. Must be a non-negative value.</param>
        /// <returns>A new rectangle that is larger by the specified amount on all sides.</returns>
        public static RawRectangleF Balloon(this RawRectangleF rectangle, float xy)
        {
            var rec = rectangle.Clone();
            rec.Left -= xy;
            rec.Top -= xy;
            rec.Right += xy;
            rec.Bottom += xy;
            return rec;
        }

        /// <summary>
        /// Expands the specified rectangle by the given amount in all directions.
        /// </summary>
        /// <remarks>This method increases the size of the rectangle by subtracting the value from the
        /// left and top edges and adding it to the right and bottom edges. The returned rectangle maintains the same
        /// center as the original.</remarks>
        /// <param name="rectangle">The rectangle to be expanded. The original rectangle is not modified.</param>
        /// <param name="xy">The number of units to expand the rectangle on each side. Must be a non-negative integer.</param>
        /// <returns>A new rectangle that is larger by the specified amount on all sides.</returns>
        public static RawRectangleF Balloon(this RawRectangleF rectangle, int xy)
        {
            var rec = rectangle.Clone();
            rec.Left -= xy;
            rec.Top -= xy;
            rec.Right += xy;
            rec.Bottom += xy;
            return rec;
        }
    }
}
