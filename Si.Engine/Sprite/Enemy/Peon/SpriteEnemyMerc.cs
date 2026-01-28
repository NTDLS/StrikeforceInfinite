using Si.Engine.Sprite.Enemy.Peon._Superclass;

namespace Si.Engine.Sprite.Enemy.Peon
{
    internal class SpriteEnemyMerc : SpriteEnemyPeonBase
    {
        public SpriteEnemyMerc(EngineCore engine, bool useDetachedMetadata = false)
            : base(engine, @"Sprites\Enemy\Peon\Merc.png", useDetachedMetadata)
        {
        }
    }
}
