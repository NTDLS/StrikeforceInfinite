using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Ae.Library.Mathematics;
using static Ae.Library.SiConstants;

namespace Ae.Engine.Sprite._Superclass.TextBlock
{
    public class SpriteRadarPositionTextBlock
        : SpriteTextBlock
    {
        public SpriteRadarPositionTextBlock(SiEngine engine, TextFormat format, SolidColorBrush color, SiVector location)
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
