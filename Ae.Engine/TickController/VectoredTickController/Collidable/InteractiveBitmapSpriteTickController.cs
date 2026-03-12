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
        /// <summary>
        /// Initializes a new instance of the InteractiveBitmapSpriteTickController class to manage interactive bitmap
        /// sprite updates within the specified engine and sprite manager.
        /// </summary>
        /// <param name="engine">The engine instance that provides the context for sprite processing and updates. Cannot be null.</param>
        /// <param name="manager">The sprite manager responsible for handling sprite objects to be controlled. Cannot be null.</param>
        public InteractiveBitmapSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        /// <summary>
        /// Advances the simulation state for all visible sprites by applying intelligence, motion, and collision
        /// detection for the current world clock tick.
        /// </summary>
        /// <remarks>This method also records multiplayer motion actions for each sprite if a multiplayer
        /// lobby is active. Call this method once per simulation tick to ensure consistent state updates.</remarks>
        /// <param name="epoch">The current simulation time, in seconds, used to update sprite behaviors and physics.</param>
        /// <param name="cameraDisplacement">The displacement vector of the camera, used to adjust sprite calculations relative to camera movement.</param>
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
