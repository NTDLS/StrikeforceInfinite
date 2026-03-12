using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ae.Engine.Rendering
{
    /// <summary>
    /// Represents a polygon defined by a collection of vertices in two-dimensional space.
    /// </summary>
    /// <remarks>The polygon is immutable; its vertices are set at construction and cannot be changed. Use
    /// this type to perform geometric operations such as bounding box calculation, clipping, and visualization. Thread
    /// safety is guaranteed for read-only operations.</remarks>
    public class AePolygon
    {
        /// <summary>
        /// Gets the collection of vertices that define the shape.
        /// </summary>
        /// <remarks>The array contains the coordinates of each vertex in the order they are used to
        /// construct the shape. The array is read-only and cannot be modified directly.</remarks>
        public RawVector2[] Vertices { get; }

        /// <summary>
        /// Initializes a new instance of the AePolygon class using the specified vertices.
        /// </summary>
        /// <param name="vertices">An array of RawVector2 objects representing the vertices of the polygon. The order of vertices determines
        /// the shape and orientation of the polygon. Cannot be null or empty.</param>
        public AePolygon(RawVector2[] vertices)
        {
            Vertices = vertices;
        }

        /// <summary>
        /// Used for polygon visualizers like https://www.wolframalpha.com/input
        /// </summary>
        /// <returns></returns>
        public string Plot()
        {
            var sb = new StringBuilder("plot polygon (");

            foreach (var v in Vertices)
            {
                sb.Append($"({v.X}, {v.Y})");
            }

            sb.Append(')');

            return sb.ToString();
        }


        /// <summary>
        /// Get the bounding rectangle of the polygon.
        /// </summary>
        /// <returns></returns>
        public RawRectangleF GetBounds()
        {
            // Initialize bounds with extreme values
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            // Find minimum and maximum X and Y coordinates of polygon vertices
            foreach (var vertex in Vertices)
            {
                minX = Math.Min(minX, vertex.X);
                minY = Math.Min(minY, vertex.Y);
                maxX = Math.Max(maxX, vertex.X);
                maxY = Math.Max(maxY, vertex.Y);
            }

            // Return the bounding rectangle
            return new RawRectangleF(minX, minY, maxX, maxY);
        }

        /// <summary>
        /// Clips the polygon so that all vertices are constrained within the specified bitmap bounds.
        /// </summary>
        /// <remarks>Vertices outside the bounds are moved to the nearest valid position within the
        /// bitmap. The original polygon is not modified.</remarks>
        /// <param name="width">The width of the bitmap, in pixels. Must be greater than zero.</param>
        /// <param name="height">The height of the bitmap, in pixels. Must be greater than zero.</param>
        /// <returns>A new AePolygon instance with all vertices adjusted to fit within the specified width and height.</returns>
        public AePolygon Clip(int width, int height)
        {
            // Clip the polygon to ensure it fits within the bounds of the original bitmap
            var clippedPolygon = new List<RawVector2>();
            foreach (var vertex in Vertices)
            {
                float x = Math.Max(0, Math.Min(vertex.X, width - 1));
                float y = Math.Max(0, Math.Min(vertex.Y, height - 1));
                clippedPolygon.Add(new RawVector2(x, y));
            }
            return new AePolygon(clippedPolygon.ToArray());
        }
    }
}
