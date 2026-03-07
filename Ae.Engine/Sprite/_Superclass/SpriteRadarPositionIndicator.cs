using Ae.Engine.Sprite._Superclass._Root;

namespace Ae.Engine.Sprite._Superclass
{
    public class SpriteRadarPositionIndicator
        : SpriteBase
    {
        public SpriteRadarPositionIndicator(SiEngine engine, string assetKey)
            : base(engine, assetKey)
        {
            X = 0;
            Y = 0;
        }
    }
}
