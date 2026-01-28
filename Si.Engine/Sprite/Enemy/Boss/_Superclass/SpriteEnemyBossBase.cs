using Si.Engine.Sprite.Enemy._Superclass;

namespace Si.Engine.Sprite.Enemy.Boss._Superclass
{
    /// <summary>
    /// Base class for "starbase" enemies.
    /// </summary>
    internal class SpriteEnemyBossBase : SpriteEnemyBase
    {
        public SpriteEnemyBossBase(EngineCore engine, string imagePath, bool useDetachedMetadata = false)
            : base(engine, imagePath, useDetachedMetadata)
        {
            RecalculateMovementVector();
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}
