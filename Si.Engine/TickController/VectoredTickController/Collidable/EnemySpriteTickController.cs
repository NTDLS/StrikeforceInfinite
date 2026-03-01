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

        /*
        #region Tightly-typed Pass through factory methods to SpriteManager.

        public SpriteEnemyBase Create(string assetKey, Action<SpriteEnemyBase>? initilizationProc = null)
            => SpriteManager.Create<SpriteEnemyBase>(assetKey, initilizationProc = null);

        public SpriteEnemyBase Add(string assetKey, Action<SpriteEnemyBase>? initilizationProc = null)
            => SpriteManager.Add<SpriteEnemyBase>(assetKey, initilizationProc);

        public void Insert(SpriteEnemyBase sprite)
             => SpriteManager.Insert(sprite);

        public SpriteEnemyBase Add(SharpDX.Direct2D1.Bitmap bitmap, Action<SpriteEnemyBase>? initilizationProc = null)
            => SpriteManager.Add<SpriteEnemyBase>(bitmap, initilizationProc);

        #endregion
        */
    }
}
