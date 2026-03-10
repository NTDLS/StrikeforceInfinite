using Ae.Engine.Sprite.Base;

namespace Ae.Engine.Sprite
{
    public class SpriteRadarPositionIndicator
        : SpriteBase
    {
        public SpriteRadarPositionIndicator(AeEngine engine, string assetKey)
            : base(engine, assetKey)
        {
            X = 0;
            Y = 0;
        }
    }
}
