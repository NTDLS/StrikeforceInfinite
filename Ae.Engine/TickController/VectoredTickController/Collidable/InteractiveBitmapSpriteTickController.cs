using Ae.Engine.Manager;
using Ae.Engine.Sprite._Superclass.Interactive;
using Ae.Engine.TickController._Superclass;
using Ae.Library.Mathematics;

namespace Ae.Engine.TickController.VectoredTickController.Collidable
{
    /// <summary>
    /// These are generic collidable, interactive bitmap sprites. They can take damage and even shoot back.
    /// </summary>
    public class InteractiveBitmapSpriteTickController
        : VectoredCollidableTickControllerBase<SpriteInteractiveBitmap>
    {
        public InteractiveBitmapSpriteTickController(SiEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector cameraDisplacement)
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
