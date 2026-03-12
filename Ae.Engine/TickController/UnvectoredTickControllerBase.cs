namespace Ae.Engine.TickController
{
    /// <summary>
    /// Tick managers that do not handle sprites or do not use a vector to update their sprites.
    /// Things like Events, Menus, Radar Position Indicators, etc.
    /// </summary>
    public class UnvectoredTickControllerBase<T>
        : ITickController<T> where T : class
    {
        /// <summary>
        /// Gets the engine instance used to execute automation tasks.
        /// </summary>
        public AeEngine Engine { get; private set; }

        /// <summary>
        /// Advances the world clock by one tick, updating time-dependent state as needed.
        /// </summary>
        /// <remarks>Override this method to implement custom logic that should occur on each world clock
        /// tick. This method is typically called by the simulation engine to synchronize time-based events.</remarks>
        public virtual void ExecuteWorldClockTick() { }

        /// <summary>
        /// Initializes a new instance of the UnvectoredTickControllerBase class using the specified engine.
        /// </summary>
        /// <param name="engine">The engine instance used to drive tick operations for this controller. Cannot be null.</param>
        public UnvectoredTickControllerBase(AeEngine engine)
        {
            Engine = engine;
        }
    }
}
