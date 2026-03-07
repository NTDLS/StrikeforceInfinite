using Ae.Library.Mathematics;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using static Ae.Library.AeConstants;

namespace Ae.Engine.Sprite._Superclass.TextBlock
{
    public class SpriteRadarPositionTextBlock
        : SpriteTextBlock
    {
        public SpriteRadarPositionTextBlock(AeEngine engine, TextFormat format, SolidColorBrush color, AeVector location)
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
