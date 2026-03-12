namespace Ae.Engine.Mathematics
{
    /// <summary>
    /// Represents a rectangle defined by its position and size using double-precision coordinates.
    /// </summary>
    /// <remarks>The rectangle is specified by its X and Y coordinates, which indicate the position of the
    /// top-left corner, and by its Width and Height. This class can be used for geometric calculations, layout, or
    /// graphical operations where precise rectangle representation is required.</remarks>
    public class AeRectangle
    {
        /// <summary>
        /// Gets or sets the X-coordinate value.
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Gets or sets the Y-coordinate value.
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// Gets or sets the width of the object.
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// Gets or sets the height value.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Initializes a new instance of the AeRectangle class.
        /// </summary>
        public AeRectangle()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AeRectangle class with the specified position and size.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle. Must be non-negative.</param>
        /// <param name="height">The height of the rectangle. Must be non-negative.</param>
        public AeRectangle(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
