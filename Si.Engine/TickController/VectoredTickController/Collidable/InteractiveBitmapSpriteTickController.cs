using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.TickController.VectoredTickController.Collidable
{
    /// <summary>
    /// These are generic collidable, interactive bitmap sprites. They can take damage and even shoot back.
    /// </summary>
    public class InteractiveBitmapSpriteTickController : VectoredCollidableTickControllerBase<SpriteInteractiveBitmap>
    {
        public InteractiveBitmapSpriteTickController(EngineCore engine, SpriteManager manager)
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
