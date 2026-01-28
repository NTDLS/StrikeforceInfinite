using Si.Engine.Sprite.Enemy.Peon._Superclass;

namespace Si.Engine.Sprite.Enemy.Peon
{
    internal class SpriteEnemyMinnow : SpriteEnemyPeonBase
    {
        public SpriteEnemyMinnow(EngineCore engine, bool useDetachedMetadata = false)
            : base(engine, @"Sprites\Enemy\Peon\Minnow.png", useDetachedMetadata)
        {
        }
    }
}
