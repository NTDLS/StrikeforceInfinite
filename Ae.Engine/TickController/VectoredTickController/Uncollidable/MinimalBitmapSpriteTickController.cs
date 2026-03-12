using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    /// <summary>
    /// These are just minimal non-collidable, non interactive, generic bitmap sprites.
    /// </summary>
    public class MinimalBitmapSpriteTickController
        : VectoredCollidableTickControllerBase<AeSpriteMinimalBitmap>
    {
        /// <summary>
        /// Initializes a new instance of the MinimalBitmapSpriteTickController class using the specified engine and
        /// sprite manager.
        /// </summary>
        /// <param name="engine">The engine instance that provides core functionality and services for sprite processing.</param>
        /// <param name="manager">The sprite manager responsible for managing sprite objects within the engine.</param>
        public MinimalBitmapSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        /// <summary>
        /// Updates the motion state of all visible sprites for the current world clock tick and records their
        /// multiplayer actions.
        /// </summary>
        /// <remarks>This method synchronizes sprite motion with the world clock and records multiplayer
        /// actions if a lobby is active. It should be called once per tick to ensure consistent state updates across
        /// all visible sprites.</remarks>
        /// <param name="epoch">The current time value, in seconds, representing the world clock tick to apply motion updates.</param>
        /// <param name="cameraDisplacement">The displacement vector of the camera, used to adjust sprite motion calculations for the current tick.</param>
        public override void ExecuteWorldClockTick(float epoch, AeVector cameraDisplacement)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyMotion(epoch, cameraDisplacement);

                Engine.MultiplayLobby?.ActionBuffer.RecordMotion(sprite.GetMultiPlayActionVector());
            }
        }
    }
}
