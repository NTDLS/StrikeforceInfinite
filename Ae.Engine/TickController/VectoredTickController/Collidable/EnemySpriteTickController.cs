using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite.Interactive.Ship;

namespace Ae.Engine.TickController.VectoredTickController.Collidable
{
    /// <summary>
    /// Provides tick-based control and management for enemy sprites within the game world, coordinating their
    /// intelligence, motion, collision detection, and resource renewal during each world clock tick.
    /// </summary>
    /// <remarks>This controller is responsible for updating all visible enemy sprites on each tick, ensuring
    /// their behaviors and state changes are synchronized with the game engine. It also records multiplayer motion
    /// actions when applicable. Use this class when you need to manage enemy sprite updates in a vectored, collidable
    /// environment.</remarks>
    public class EnemySpriteTickController
        : VectoredCollidableTickControllerBase<AeSpriteEnemy>
    {
        /// <summary>
        /// Initializes a new instance of the EnemySpriteTickController class using the specified engine and sprite
        /// manager.
        /// </summary>
        /// <param name="engine">The engine instance that provides core game functionality and services required by the controller. Cannot be
        /// null.</param>
        /// <param name="manager">The sprite manager responsible for managing enemy sprite objects. Cannot be null.</param>
        public EnemySpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        /// <summary>
        /// Updates all visible sprites for the current world clock tick, applying intelligence, motion, collision
        /// detection, and resource renewal.
        /// </summary>
        /// <remarks>This method also records multiplayer motion actions for each sprite if a multiplayer
        /// lobby is active. Call this method once per simulation tick to ensure consistent sprite updates.</remarks>
        /// <param name="epoch">The current time value, in seconds, representing the world clock tick to process.</param>
        /// <param name="cameraDisplacement">The displacement vector of the camera, used to adjust sprite behavior and motion calculations.</param>
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
    }
}
