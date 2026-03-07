using Ae.Engine.Manager;
using Ae.Engine.Sprite._Superclass.Interactive.Ship;
using Ae.Engine.TickController._Superclass;
using Ae.Library.Mathematics;

namespace Ae.Engine.TickController.VectoredTickController.Collidable
{
    public class EnemySpriteTickController
        : VectoredCollidableTickControllerBase<SpriteEnemy>
    {
        private readonly SiEngine _engine;

        public EnemySpriteTickController(SiEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
            _engine = engine;
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector cameraDisplacement)
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
