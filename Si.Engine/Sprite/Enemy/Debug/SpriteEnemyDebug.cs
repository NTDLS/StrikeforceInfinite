using Si.Engine.AI.Logistics;
using Si.Engine.Sprite.Enemy.Peon._Superclass;

namespace Si.Engine.Sprite.Enemy.Debug
{
    /// <summary>
    /// Debugging enemy unit - a scary sight to see.
    /// </summary>
    internal class SpriteEnemyDebug : SpriteEnemyPeonBase
    {
        public SpriteEnemyDebug(EngineCore engine, string spritePath)
            : base(engine, spritePath)
        {
            AddAIController(new AILogisticsHostileEngagement(_engine, this, [_engine.Player.Sprite]));
            SetCurrentAIController<AILogisticsHostileEngagement>();
        }
    }
}

