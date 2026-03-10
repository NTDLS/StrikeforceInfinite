using Ae.Engine.Sprite.Base;

namespace Ae.Engine.Sprite
{
    public class AeSpriteRadarPositionIndicator
        : AeSprite
    {
        public AeSpriteRadarPositionIndicator(AeEngine engine, string assetKey)
            : base(engine, assetKey)
        {
            X = 0;
            Y = 0;
        }
    }
}
