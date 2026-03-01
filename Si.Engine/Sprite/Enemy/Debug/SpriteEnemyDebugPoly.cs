using Si.Engine.AI.Logistics;
using Si.Engine.Sprite.Enemy.Peon._Superclass;

namespace Si.Engine.Sprite.Enemy.Debug
{
    /// <summary>
    /// Debugging enemy unit - a scary sight to see.
    /// </summary>
    internal class SpriteEnemyDebugPoly : SpriteEnemyPeonBase
    {
        public SpriteEnemyDebugPoly(EngineCore engine, string assetKey)
            : base(engine, assetKey)
        {
            AddAIController(new AILogisticsHostileEngagement(_engine, this, [_engine.Player.Sprite]));
            SetCurrentAIController<AILogisticsHostileEngagement>();
        }
    }
}

