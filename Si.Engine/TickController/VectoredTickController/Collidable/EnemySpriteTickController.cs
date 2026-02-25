using Si.Engine.Manager;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.TickController.VectoredTickController.Collidable
{
    public class EnemySpriteTickController
        : VectoredCollidableTickControllerBase<SpriteEnemyBase>
    {
        private readonly EngineCore _engine;

        public EnemySpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
            _engine = engine;
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyIntelligence(epoch, displacementVector);
                sprite.ApplyMotion(epoch, displacementVector);
                sprite.PerformCollisionDetection(epoch);
                sprite.RenewableResources.RenewAllResources(epoch);

                Engine.MultiplayLobby?.ActionBuffer.RecordMotion(sprite.GetMultiPlayActionVector());
            }
        }

        public new SpriteEnemyBase Add(string spritePath)
            => _engine.Sprites.Add<SpriteEnemyBase>(spritePath);
    }
}
