using Ae.Engine.Mathematics;

namespace Ae.Engine.TickController
{
    /// <summary>
    /// Tick manager that generates offset vectors for the one and only local player sprite.
    /// </summary>
    public class PlayerSpriteTickControllerBase<T> : ITickController<T> where T : class
    {
        /// <summary>
        /// Gets the engine instance used to execute automation tasks.
        /// </summary>
        public AeEngine Engine { get; private set; }

        /// <summary>
        /// Moves the player and returns the direction and amount of movement which was applied.
        /// </summary>
        /// <returns>Returns the direction and amount of movement that the player has moved in the current tick.</returns>
        public virtual AeVector ExecuteWorldClockTick(float epochTime) => new();

        /// <summary>
        /// Initializes a new instance of the PlayerSpriteTickControllerBase class using the specified engine.
        /// </summary>
        /// <param name="engine">The engine instance used to control player sprite ticks. Cannot be null.</param>
        public PlayerSpriteTickControllerBase(AeEngine engine)
        {
            Engine = engine;
        }
    }
}
