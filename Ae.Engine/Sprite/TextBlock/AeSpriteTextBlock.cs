using Ae.Engine.Mathematics;
using Ae.Engine.Sprite.Base;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System.Drawing;

namespace Ae.Engine.Sprite.TextBlock
{
    /// <summary>
    /// Represents a text block sprite that displays formatted text within a specified area on the screen.
    /// </summary>
    /// <remarks>AeSpriteTextBlock provides functionality for rendering text with custom formatting and color,
    /// and supports centering operations along the X and Y axes. The text block's size is automatically updated based
    /// on the text content and formatting. Use this type when you need to display text as a sprite with precise layout
    /// and rendering control.</remarks>
    public class AeSpriteTextBlock
        : AeSprite
    {
        private Size _size = Size.Empty;
        private string _text = string.Empty;

        #region Properties.

        /// <summary>
        /// Non-sprites (e.g. only text) bounds are simple, unlike sprites the text bounds start at X,Y and go to Width/Height.
        /// </summary>
        public override RectangleF Bounds => new(Location.X, Location.Y, Size.Width, Size.Height);

        /// <summary>
        /// Gets the bounding rectangle of the element in raw coordinates.
        /// </summary>
        public override RawRectangleF RawBounds => new(Location.X, Location.Y, Location.X + Size.Width, Location.Y + Size.Height);

        /// <summary>
        /// Gets the rectangular area, in device-independent pixels, that represents the bounds where the element is
        /// rendered.
        /// </summary>
        public override RectangleF RenderBounds => new(RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height);

        /// <summary>
        /// Gets the raw bounding rectangle of the rendered content in device-independent coordinates.
        /// </summary>
        /// <remarks>Use this property to determine the exact area occupied by the rendered element. The
        /// coordinates are relative to the element's render location and size.</remarks>
        public override RawRectangleF RawRenderBounds => new(RenderLocation.X, RenderLocation.Y, RenderLocation.X + Size.Width, RenderLocation.Y + Size.Height);

        /// <summary>
        /// Gets or sets the text formatting options applied to the content.
        /// </summary>
        public TextFormat Format { get; set; }

        /// <summary>
        /// Gets the brush used to render the color of the element.
        /// </summary>
        public SolidColorBrush Color { get; private set; }

        /// <summary>
        /// Gets the height component of the size represented by this instance.
        /// </summary>
        public float Height => _size.Height;

        /// <summary>
        /// Gets the size of the element as a <see cref="Size"/> structure.
        /// </summary>
        public override Size Size => _size;

        /// <summary>
        /// Gets or sets the text. On set, the size is recalculated.
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                var size = Engine.Rendering.GetTextSize(_text, Format);
                _size = new Size((int)size.Width, (int)size.Height);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the AeSpriteTextBlock class with the specified text format, color, location,
        /// and position mode.
        /// </summary>
        /// <param name="engine">The engine instance used to manage rendering and resources for the sprite text block.</param>
        /// <param name="format">The text format to apply to the sprite text block, including font and style settings.</param>
        /// <param name="color">The brush used to render the text color for the sprite text block.</param>
        /// <param name="location">The initial location of the sprite text block within the rendering coordinate space.</param>
        /// <param name="isFixedPosition">A value indicating whether the sprite text block maintains a fixed position relative to the screen (<see
        /// langword="true"/>), or moves with the scene (<see langword="false"/>).</param>
        public AeSpriteTextBlock(AeEngine engine, TextFormat format, SolidColorBrush color, AeVector location, bool isFixedPosition)
            : base(engine, null)
        {
            RenderScaleOrder = AeRenderScaleOrder.PostScale;
            IsFixedPosition = isFixedPosition;
            Location = new AeVector(location);
            Color = color;

            Format = format;
        }

        /// <summary>
        /// Centers the object horizontally and vertically within the natural screen size.
        /// </summary>
        /// <remarks>This method updates the object's X and Y coordinates so that it is positioned at the
        /// center of the display area, based on its current size. Use this method when you want the object to appear
        /// centered regardless of screen resolution.</remarks>
        public void CenterXY()
        {
            X = Engine.Display.NaturalScreenSize.Width / 2 - Size.Width / 2;
            Y = Engine.Display.NaturalScreenSize.Height / 2 - Size.Height / 2;
        }

        /// <summary>
        /// Centers the Y coordinate of the object within the natural screen height.
        /// </summary>
        /// <remarks>This method adjusts the object's vertical position so that it is centered relative to
        /// the display's natural screen size. It is useful for aligning objects in the middle of the screen, regardless
        /// of their height.</remarks>
        public void CenterY()
        {
            Y = Engine.Display.NaturalScreenSize.Height / 2 - Size.Height / 2;
        }

        /// <summary>
        /// Centers the object horizontally within the natural screen area.
        /// </summary>
        /// <remarks>This method adjusts the object's X coordinate so that it is positioned in the
        /// horizontal center of the display, based on its current width and the screen's natural size. Use this method
        /// when you want the object to appear centered regardless of screen resolution.</remarks>
        public void CenterX()
        {
            X = (Engine.Display.NaturalScreenSize.Width / 2) - (Size.Width / 2);
        }

        /// <summary>
        /// Sets the displayed text and centers the object horizontally and vertically within the natural screen size.
        /// </summary>
        /// <remarks>This method updates both the text and the object's position. The centering
        /// calculation uses the current size of the object and the natural screen dimensions. Call this method after
        /// updating the object's size or text to ensure correct centering.</remarks>
        /// <param name="text">The text to display. If null or empty, the object will be centered with no visible text.</param>
        public void SetTextAndCenterXY(string text)
        {
            Text = text;
            X = Engine.Display.NaturalScreenSize.Width / 2 - Size.Width / 2;
            Y = Engine.Display.NaturalScreenSize.Height / 2 - Size.Height / 2;
        }

        /// <summary>
        /// Sets the displayed text and vertically centers the element within the natural screen height.
        /// </summary>
        /// <remarks>This method updates both the text and the vertical position of the element. The
        /// element will be centered based on its current height and the natural screen size. If the text changes the
        /// element's height, subsequent calls may be needed to maintain centering.</remarks>
        /// <param name="text">The text to display. Cannot be null.</param>
        public void SetTextAndCenterY(string text)
        {
            Text = text;
            Y = Engine.Display.NaturalScreenSize.Height / 2 - Size.Height / 2;
        }

        /// <summary>
        /// Sets the displayed text and centers the element horizontally within the natural screen size.
        /// </summary>
        /// <remarks>This method updates both the text and the horizontal position of the element. The
        /// element will be centered based on its current width and the natural screen size. If the text changes the
        /// element's width, the centering will reflect the new size.</remarks>
        /// <param name="text">The text to display. Cannot be null.</param>
        public void SetTextAndCenterX(string text)
        {
            Text = text;
            X = Engine.Display.NaturalScreenSize.Width / 2 - Size.Width / 2;
        }

        internal override void Render(RenderTarget renderTarget, float epoch)
        {
            if (IsVisible)
            {
                Engine.Rendering.DrawText(renderTarget,
                    RenderLocation.X,
                    RenderLocation.Y,
                    0, _text ?? string.Empty, Format, Color);
            }
        }
    }
}