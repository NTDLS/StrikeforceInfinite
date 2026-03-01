using Si.Engine.Sprite._Superclass._Root;

namespace Si.Engine.Sprite
{
    public class SpriteRadarPositionIndicator
        : SpriteBase
    {
        public SpriteRadarPositionIndicator(EngineCore engine, string assetKey)
            : base(engine, assetKey)
        {
            X = 0;
            Y = 0;
        }
    }
}
