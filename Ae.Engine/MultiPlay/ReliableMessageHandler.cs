using NTDLS.ReliableMessaging;

namespace Ae.Engine.MultiPlay
{
    /// <summary>
    /// Provides functionality for handling messages with reliability guarantees within the messaging engine.
    /// </summary>
    /// <remarks>This class implements the IRmMessageHandler interface to support reliable message processing
    /// scenarios. It is intended for internal use within the messaging infrastructure and is not designed for direct
    /// consumption by external callers.</remarks>
    internal class ReliableMessageHandler(AeEngine engine)
        : IRmMessageHandler
    {
    }
}
