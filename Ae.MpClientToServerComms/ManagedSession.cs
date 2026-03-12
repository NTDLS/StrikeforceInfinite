using System;
using System.Net;

namespace Ae.MpClientToServerComms
{
    /// <summary>
    /// An instance of a server session.
    /// </summary>
    public class ManagedSession
    {
        /// <summary>
        /// Gets the unique identifier for the current session.
        /// </summary>
        public Guid SessionId { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Id of the reliable messaging connection.
        /// </summary>
        public Guid ConnectionId { get; private set; }

        /// <summary>
        /// Endpoint of the client that this session is associated with.
        /// </summary>
        public IPEndPoint? DatagramEndPoint { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public ManagedSession(Guid connectionId)
        {
            ConnectionId = connectionId;
        }

        /// <summary>
        /// Associates the specified datagram endpoint with the current instance.
        /// </summary>
        /// <param name="ipEndPoint">The datagram endpoint to attach. May be null to detach any existing endpoint.</param>
        public void AttachDatagramEndpoint(IPEndPoint? ipEndPoint)
        {
            DatagramEndPoint = ipEndPoint;
        }
    }
}
