using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite.Interactive;

namespace Ae.Engine.TickController.VectoredTickController.Collidable
{
    /// <summary>
    /// These are generic collidable, interactive bitmap sprites. They can take damage and even shoot back.
    /// </summary>
    public class InteractiveBitmapSpriteTickController
        : VectoredCollidableTickControllerBase<AeSpriteInteractiveBitmap>
    {
        public InteractiveBitmapSpriteTickController(AeEngine engine, SpriteManager manager)
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
