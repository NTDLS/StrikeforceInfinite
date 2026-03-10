using Ae.Engine.Mathematics;
using SharpDX.Mathematics.Interop;

namespace Ae.Engine.Rendering
{
    public static class RawRectangleExtensions
    {
        /// <summary>
        /// Clones a float rectangle.
        /// </summary>
        public static RawRectangleF Clone(this RawRectangleF rectangle)
        {
            return new RawRectangleF(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
        }

        public static RawRectangleF Balloon(this RawRectangleF rectangle, AeVector size)
        {
            var rec = rectangle.Clone();
            rec.Left -= size.X;
            rec.Top -= size.Y;
            rec.Right += size.X;
            rec.Bottom += size.Y;
            return rec;
        }

        public static RawRectangleF Balloon(this RawRectangleF rectangle, float x, float y)
        {
            var rec = rectangle.Clone();
            rec.Left -= x;
            rec.Top -= y;
            rec.Right += x;
            rec.Bottom += y;
            return rec;
        }

        public static RawRectangleF Balloon(this RawRectangleF rectangle, int x, int y)
        {
            var rec = rectangle.Clone();
            rec.Left -= x;
            rec.Top -= y;
            rec.Right += x;
            rec.Bottom += y;
            return rec;
        }

        public static RawRectangleF Balloon(this RawRectangleF rectangle, float xy)
        {
            var rec = rectangle.Clone();
            rec.Left -= xy;
            rec.Top -= xy;
            rec.Right += xy;
            rec.Bottom += xy;
            return rec;
        }

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
