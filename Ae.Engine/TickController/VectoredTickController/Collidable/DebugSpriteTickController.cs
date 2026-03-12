using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite.Interactive.Ship;

namespace Ae.Engine.TickController.VectoredTickController.Collidable
{
    /// <summary>
    /// Provides tick-based control for debug sprites, managing their intelligence, motion, and collision detection
    /// during each world clock tick.
    /// </summary>
    /// <remarks>This controller is intended for use with debug sprites in environments where vectored
    /// collision and multiplayer action recording are required. It integrates with the engine's multiplayer lobby to
    /// record sprite motion actions, enabling synchronized state across clients. Use this controller when you need to
    /// simulate or debug sprite behavior with full tick lifecycle management.</remarks>
    public class DebugSpriteTickController
        : VectoredCollidableTickControllerBase<AeSpriteDebug>
    {
        /// <summary>
        /// Initializes a new instance of the DebugSpriteTickController class for managing debug sprite updates within
        /// the specified engine and sprite manager.
        /// </summary>
        /// <param name="engine">The engine instance that provides the context for sprite operations. Cannot be null.</param>
        /// <param name="manager">The sprite manager responsible for handling sprite entities. Cannot be null.</param>
        public DebugSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        /// <summary>
        /// Updates all visible sprites for the current world clock tick, applying intelligence, motion, and collision
        /// detection, and records multiplayer motion actions.
        /// </summary>
        /// <remarks>This method processes each visible sprite in sequence, ensuring their state is
        /// updated for the current tick. Multiplayer motion actions are recorded if a lobby is active.</remarks>
        /// <param name="epoch">The current time epoch, used to synchronize sprite updates and actions.</param>
        /// <param name="cameraDisplacement">The displacement vector of the camera, used to adjust sprite behavior and motion relative to the camera's
        /// position.</param>
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
