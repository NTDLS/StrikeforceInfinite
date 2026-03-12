namespace Ae.Engine.AI
{
    /// <summary>
    /// Represents an AI state handler.
    /// </summary>
    public interface IAeAIStateHandler
    {
        /// <summary>
        /// This method is called by the engine at regular intervals to allow the AI state handler
        /// to perform its logic and update its state based on the elapsed time (epoch).
        /// </summary>
        void Tick(float epoch);
    }
}
