using Ae.Engine.Mathematics;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Sprite.TextBlock
{
    public class AeSpriteRadarPositionTextBlock
        : AeSpriteTextBlock
    {
        public AeSpriteRadarPositionTextBlock(AeEngine engine, TextFormat format, SolidColorBrush color, AeVector location)
            : base(engine, format, color, location, false)
        {
            RenderScaleOrder = SiRenderScaleOrder.PreScale;
            IsVisible = false;
        }

        private float _distanceValue;
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
