using System.Threading;

namespace Ae.Engine.Helpers
{
    /// <summary>
    /// Provides a thread-safe mechanism for generating unique sequential identifiers.
    /// </summary>
    /// <remarks>This class is commonly used to assign unique IDs to objects, such as sprites, for debugging
    /// or tracking purposes. The generated IDs are guaranteed to be unique within the application's lifetime and are
    /// incremented atomically to ensure thread safety.</remarks>
    public static class AeSequenceGenerator
    {
        private static uint _nextSequentialId = 0;
        /// <summary>
        /// Used to give all loaded sprites a unique ID. Very handy for debugging.
        /// </summary>
        public static uint Next() => Interlocked.Increment(ref _nextSequentialId);
    }
}
