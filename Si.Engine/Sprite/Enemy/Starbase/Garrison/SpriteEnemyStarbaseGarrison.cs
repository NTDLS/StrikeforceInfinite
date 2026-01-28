using Si.Engine;
using Si.Engine.Sprite.Enemy.Starbase._Superclass;
using Si.Library.Mathematics;

namespace Si.GameEngine.Sprite.Enemy.Starbase.Garrison
{
    internal class SpriteEnemyStarbaseGarrison(EngineCore engine, bool useDetachedMetadata = false)
        : SpriteEnemyStarbase(engine, @"Sprites\Enemy\Starbase\Garrison\Hull.png", useDetachedMetadata)
    {
        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            //Orientation.Degrees += 0.005f;
            base.ApplyMotion(epoch, displacementVector);
        }
    }
}

