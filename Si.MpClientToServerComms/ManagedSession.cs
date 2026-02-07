using System.Net;

namespace Si.MpClientToServerComms
{
    /// <summary>
    /// An instance of a server session.
    /// </summary>
    public class ManagedSession
    {
        public Guid SessionId { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Id of the reliable messaging connection.
        /// </summary>
        public Guid ConnectionId { get; private set; }

        /// <summary>
        /// Endpoint of the client that this session is associated with.
        /// </summary>
        public IPEndPoint? DatagramEndPoint { get; private set; }

        public ManagedSession(Guid connectionId)
        {
            ConnectionId = connectionId;
        }

        public void AttachDatagramEndpoint(IPEndPoint? ipEndPoint)
        {
            DatagramEndPoint = ipEndPoint;
        }
    }
}
