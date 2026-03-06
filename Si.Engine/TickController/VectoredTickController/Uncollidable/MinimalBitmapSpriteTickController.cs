using Si.Engine.Manager;
using Si.Engine.Sprite._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.TickController.VectoredTickController.Uncollidable
{
    /// <summary>
    /// These are just minimal non-collidable, non interactive, generic bitmap sprites.
    /// </summary>
    public class MinimalBitmapSpriteTickController
        : VectoredCollidableTickControllerBase<SpriteMinimalBitmap>
    {
        public MinimalBitmapSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector cameraDisplacement)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyMotion(epoch, cameraDisplacement);

                Engine.MultiplayLobby?.ActionBuffer.RecordMotion(sprite.GetMultiPlayActionVector());
            }
        }
    }
}
