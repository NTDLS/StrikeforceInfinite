using System.Drawing;

namespace Ae.Engine.Mathematics
{
    /// <summary>
    /// Represents a spatial quadrant defined by a unique key and bounding rectangle.
    /// </summary>
    /// <remarks>The quadrant is typically used in spatial partitioning scenarios, such as quadtrees or
    /// grid-based spatial indexing, to organize and reference regions within a two-dimensional space. The key
    /// identifies the quadrant, while the bounds specify its area.</remarks>
    public class AeQuadrant
    {
        /// <summary>
        /// Gets the point that serves as the key for this entry.
        /// </summary>
        public Point Key { get; private set; }

        /// <summary>
        /// Gets the bounding rectangle that defines the position and size of the object.
        /// </summary>
        public Rectangle Bounds { get; private set; }

        /// <summary>
        /// Initializes a new instance of the AeQuadrant class with the specified key and bounds.
        /// </summary>
        /// <param name="key">The point that uniquely identifies the quadrant within the spatial structure.</param>
        /// <param name="bounds">The rectangle that defines the spatial boundaries of the quadrant.</param>
        public AeQuadrant(Point key, Rectangle bounds)
        {
            Key = key;
            Bounds = bounds;
        }

        /// <summary>
        /// Returns a string representation of the current object.
        /// </summary>
        /// <returns>A string that represents the value of the Key property.</returns>
        public override string ToString() => Key.ToString();
    }
}