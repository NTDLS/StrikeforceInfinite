using Si.Engine.Sprite.Enemy._Superclass;

namespace Si.Engine.Sprite.Enemy.Debug
{
    /// <summary>
    /// Debugging enemy unit - a scary sight to see.
    /// </summary>
    internal class SpriteEnemyDebugStatic(EngineCore engine, string assetKey)
        : SpriteEnemy(engine, assetKey)
    {
        public override void OnMaterialized()
        {
            base.OnMaterialized();
        }
    }
}
