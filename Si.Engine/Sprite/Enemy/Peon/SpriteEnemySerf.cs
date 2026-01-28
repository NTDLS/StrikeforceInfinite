using Si.Engine.Sprite.Enemy.Peon._Superclass;

namespace Si.Engine.Sprite.Enemy.Peon
{
    internal class SpriteEnemySerf : SpriteEnemyPeonBase
    {
        public SpriteEnemySerf(EngineCore engine, bool useDetachedMetadata = false)
            : base(engine, @"Sprites\Enemy\Peon\Serf.png", useDetachedMetadata)
        {
        }
    }
}
