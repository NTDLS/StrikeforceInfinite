using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite.Interactive.Ship;

namespace Ae.Engine.TickController.VectoredTickController.Collidable
{
    public class EnemySpriteTickController
        : VectoredCollidableTickControllerBase<SpriteEnemy>
    {
        private readonly AeEngine _engine;

        public EnemySpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
            _engine = engine;
        }

        public override void ExecuteWorldClockTick(float epoch, AeVector cameraDisplacement)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyIntelligence(epoch, cameraDisplacement);
                sprite.ApplyMotion(epoch, cameraDisplacement);
                sprite.PerformCollisionDetection(epoch);
                sprite.RenewableResources.RenewAllResources(epoch);

                Engine.MultiplayLobby?.ActionBuffer.RecordMotion(sprite.GetMultiPlayActionVector());
            }
        }

        /*
        #region Tightly-typed Pass through factory methods to SpriteManager.

        public SpriteEnemyBase Create(string assetKey, Action<SpriteEnemyBase>? initializationProc = null)
            => SpriteManager.Create<SpriteEnemyBase>(assetKey, initializationProc);

        public SpriteEnemyBase Add(string assetKey, Action<SpriteEnemyBase>? initializationProc = null)
            => SpriteManager.Add<SpriteEnemyBase>(assetKey, initializationProc);

        public void Insert(SpriteEnemyBase sprite)
             => SpriteManager.Insert(sprite);

        public SpriteEnemyBase Add(SharpDX.Direct2D1.Bitmap bitmap, Action<SpriteEnemyBase>? initializationProc = null)
            => SpriteManager.Add<SpriteEnemyBase>(bitmap, initializationProc);

        #endregion
        */
    }
}
