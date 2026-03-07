using Ae.Engine.Manager;
using Ae.Engine.Sprite._Superclass.Interactive.Ship;
using Ae.Engine.TickController._Superclass;
using Ae.Library.Mathematics;

namespace Ae.Engine.TickController.VectoredTickController.Collidable
{
    public class DebugSpriteTickController
        : VectoredCollidableTickControllerBase<SpriteDebug>
    {
        public DebugSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, AeVector cameraDisplacement)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyIntelligence(epoch, cameraDisplacement);
                sprite.ApplyMotion(epoch, cameraDisplacement);
                sprite.PerformCollisionDetection(epoch);

                Engine.MultiplayLobby?.ActionBuffer.RecordMotion(sprite.GetMultiPlayActionVector());
            }
        }
    }
}
