using Si.Engine.Sprite.Enemy.Peon._Superclass;

namespace Si.Engine.Sprite.Enemy.Peon
{
    internal class SpriteEnemyScav : SpriteEnemyPeonBase
    {
        public SpriteEnemyScav(EngineCore engine, bool useDetachedMetadata = false)
            : base(engine, @"Sprites\Enemy\Peon\Scav.png", useDetachedMetadata)
        {
        }
    }
}
