using System.Drawing;

namespace Ae.Engine.Mathematics
{
    public class AeQuadrant
    {
        public Point Key { get; private set; }
        public Rectangle Bounds { get; private set; }

        public AeQuadrant(Point key, Rectangle bounds)
        {
            Key = key;
            Bounds = bounds;
        }

        public override string ToString() => Key.ToString();
    }
}