using Ae.Engine.Mathematics;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

namespace Ae.Engine.Sprite.TextBlock
{
    /// <summary>
    /// Represents a text block that displays the radar position distance in a sprite-based user interface.
    /// </summary>
    /// <remarks>This class is intended for use within radar overlays or HUD elements where the distance to a
    /// target or object is shown. The text block is initially hidden and uses a pre-scale rendering order for
    /// consistent appearance. The displayed text is automatically updated when the distance value changes.</remarks>
    public class AeSpriteRadarPositionTextBlock
        : AeSpriteTextBlock
    {
        /// <summary>
        /// Initializes a new instance of the AeSpriteRadarPositionTextBlock class with the specified engine, text
        /// format, color, and location.
        /// </summary>
        /// <param name="engine">The engine instance used to manage rendering and game logic for the text block.</param>
        /// <param name="format">The text format applied to the displayed text, including font and style settings.</param>
        /// <param name="color">The brush used to render the text color.</param>
        /// <param name="location">The position of the text block within the radar display.</param>
        public AeSpriteRadarPositionTextBlock(AeEngine engine, TextFormat format, SolidColorBrush color, AeVector location)
            : base(engine, format, color, location, false)
        {
            RenderScaleOrder = AeRenderScaleOrder.PreScale;
            IsVisible = false;
        }

        private float _distanceValue;
        /// <summary>
        /// Gets or sets the distance value displayed by the control.
        /// </summary>
        /// <remarks>Setting this property updates the displayed text to reflect the new value, formatted
        /// with digit grouping. The value is not validated; callers should ensure it represents a meaningful distance
        /// as required by their application.</remarks>
        public float DistanceValue
        {
            get
            {
                return _distanceValue;
            }
            set
            {
                _distanceValue = value;
                Text = DistanceValue.ToString("#,#");
            }
        }
    }
}
