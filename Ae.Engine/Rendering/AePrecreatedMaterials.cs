using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Ae.Engine.Rendering
{
    /// <summary>
    /// Provides access to a predefined set of color values and corresponding solid color brushes for use in rendering
    /// operations.
    /// </summary>
    /// <remarks>The class exposes commonly used colors and their associated brushes, allowing consistent
    /// usage throughout rendering code. Brushes are initialized for a given render target and are intended for reuse to
    /// improve performance and maintain visual consistency.</remarks>
    public class AePrecreatedMaterials
    {
        /// <summary>
        /// Provides a set of predefined colors represented as RGBA values for use in rendering and graphics operations.
        /// </summary>
        /// <remarks>The colors exposed by this class include common primary colors, grayscale shades, and
        /// additional colors suitable for UI elements and backgrounds. Each color is provided as a read-only property
        /// of type RawColor4, which encapsulates the red, green, blue, and alpha components. This class is intended to
        /// simplify access to standard color values and promote consistency across graphical applications.</remarks>
        public class RawColors
        {
            /// <summary>
            /// Gets the raw color value representing pure red with full opacity.
            /// </summary>
            public RawColor4 Red { get; private set; } = new(1, 0, 0, 1);
            /// <summary>
            /// Gets the raw color value representing pure green in RGBA format.
            /// </summary>
            public RawColor4 Green { get; private set; } = new(0, 1, 0, 1);
            /// <summary>
            /// Gets the color representing pure blue in RGBA format.
            /// </summary>
            public RawColor4 Blue { get; private set; } = new(0, 0, 1, 1);
            /// <summary>
            /// Gets the raw color value representing opaque black.
            /// </summary>
            public RawColor4 Black { get; private set; } = new(0, 0, 0, 1);
            /// <summary>
            /// Gets the color value representing pure white in RGBA format.
            /// </summary>
            public RawColor4 White { get; private set; } = new(1, 1, 1, 1);
            /// <summary>
            /// Gets the background color used for the editor interface.
            /// </summary>
            public RawColor4 EditorBackground { get; private set; } = new(30 / 255.0f, 50 / 255.0f, 40 / 255.0f, 1);
            /// <summary>
            /// Gets the raw color value representing a standard gray shade.
            /// </summary>
            public RawColor4 Gray { get; private set; } = new(0.25f, 0.25f, 0.25f, 1);
            /// <summary>
            /// Gets the color that represents white smoke.
            /// </summary>
            public RawColor4 WhiteSmoke { get; private set; } = new(0.9608f, 0.9608f, 0.9608f, 1);
            /// <summary>
            /// Gets the color representing cyan in RGBA format.
            /// </summary>
            public RawColor4 Cyan { get; private set; } = new(0, 1f, 1f, 1f);
            /// <summary>
            /// Gets the color that represents OrangeRed in the RGBA color space.
            /// </summary>
            public RawColor4 OrangeRed { get; private set; } = new(0.9f, 0.2706f, 0.0000f, 1);
            /// <summary>
            /// Gets the RGBA color value representing orange.
            /// </summary>
            public RawColor4 Orange { get; private set; } = new(1f, 0.6471f, 0.0f, 1);
            /// <summary>
            /// Gets the ARGB color value representing Lawn Green.
            /// </summary>
            public RawColor4 LawnGreen { get; private set; } = new(0.4863f, 0.9882f, 0f, 1);
            /// <summary>
            /// Gets the fully transparent color represented as a raw RGBA value.
            /// </summary>
            public RawColor4 Transparent { get; private set; } = new(0, 0, 0, 0);
        }

        /// <summary>
        /// Provides a collection of commonly used solid color brushes for rendering operations.
        /// </summary>
        /// <remarks>The brushes are initialized using the specified render target and color definitions.
        /// Use these brushes to simplify drawing with standard colors in graphics applications. All brushes are
        /// read-write within the assembly, but are intended to be used as pre-defined resources for consistent color
        /// usage.</remarks>
        public class ColorBrushes
        {
            /// <summary>
            /// Gets the brush representing the color red.
            /// </summary>
            public SolidColorBrush Red { get; internal set; }
            /// <summary>
            /// Gets the brush that represents the green color.
            /// </summary>
            public SolidColorBrush Green { get; internal set; }
            /// <summary>
            /// Gets the brush representing the color blue.
            /// </summary>
            public SolidColorBrush Blue { get; internal set; }
            /// <summary>
            /// Gets the solid black brush used for drawing or filling areas with the color black.
            /// </summary>
            public SolidColorBrush Black { get; internal set; }
            /// <summary>
            /// Gets the solid white brush used for painting areas with a pure white color.
            /// </summary>
            public SolidColorBrush White { get; internal set; }
            /// <summary>
            /// Gets the brush representing a standard gray color used for UI elements.
            /// </summary>
            public SolidColorBrush Gray { get; internal set; }
            /// <summary>
            /// Gets or sets the brush that represents the WhiteSmoke color.
            /// </summary>
            public SolidColorBrush WhiteSmoke { get; internal set; }
            /// <summary>
            /// Gets the brush that represents the cyan color.
            /// </summary>
            public SolidColorBrush Cyan { get; internal set; }
            /// <summary>
            /// Gets the brush that represents the OrangeRed color.
            /// </summary>
            public SolidColorBrush OrangeRed { get; internal set; }
            /// <summary>
            /// Gets the brush representing the orange color used for UI elements.
            /// </summary>
            public SolidColorBrush Orange { get; internal set; }
            /// <summary>
            /// Gets the brush that paints a LawnGreen color.
            /// </summary>
            public SolidColorBrush LawnGreen { get; internal set; }
            /// <summary>
            /// Gets the brush that represents a fully transparent color.
            /// </summary>
            public SolidColorBrush Transparent { get; internal set; }

            /// <summary>
            /// Initializes a set of predefined solid color brushes for use with the specified render target and color
            /// palette.
            /// </summary>
            /// <remarks>Each brush corresponds to a commonly used color and can be used for drawing
            /// operations on the provided render target. The brushes are created using the color values from the
            /// specified palette.</remarks>
            /// <param name="renterTarget">The render target to which the brushes will be associated. Cannot be null.</param>
            /// <param name="color">The collection of raw color values used to create each brush. Cannot be null.</param>
            public ColorBrushes(RenderTarget renterTarget, RawColors color)
            {
                Red = new SolidColorBrush(renterTarget, color.Red);
                Green = new SolidColorBrush(renterTarget, color.Green);
                Blue = new SolidColorBrush(renterTarget, color.Blue);
                Black = new SolidColorBrush(renterTarget, color.Black);
                White = new SolidColorBrush(renterTarget, color.White);
                Gray = new SolidColorBrush(renterTarget, color.Gray);
                WhiteSmoke = new SolidColorBrush(renterTarget, color.WhiteSmoke);
                Cyan = new SolidColorBrush(renterTarget, color.Cyan);
                OrangeRed = new SolidColorBrush(renterTarget, color.OrangeRed);
                Orange = new SolidColorBrush(renterTarget, color.Orange);
                LawnGreen = new SolidColorBrush(renterTarget, color.LawnGreen);
                Transparent = new SolidColorBrush(renterTarget, color.Transparent);
            }
        }

        /// <summary>
        /// Gets the collection of predefined color brushes available for use in drawing operations.
        /// </summary>
        public ColorBrushes Brushes { get; private set; }

        /// <summary>
        /// Gets the collection of raw color values associated with the current instance.
        /// </summary>
        public RawColors Colors { get; private set; } = new();

        internal AePrecreatedMaterials(RenderTarget renterTarget)
        {
            Brushes = new ColorBrushes(renterTarget, Colors);
        }
    }
}
