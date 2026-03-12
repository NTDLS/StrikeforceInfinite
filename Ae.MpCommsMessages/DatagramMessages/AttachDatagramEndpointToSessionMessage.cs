using NTDLS.DatagramMessaging;
using System;

namespace Ae.MpCommsMessages.DatagramMessages
{
    /// <summary>
    /// Represents a message used to attach a datagram endpoint to an existing session.
    /// </summary>
    public class AttachDatagramEndpointToSessionMessage
        : IDmDatagram
    {
        /// <summary>
        /// Gets or sets the unique identifier for the current session.
        /// </summary>
        public Guid SessionId { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AttachDatagramEndpointToSessionMessage()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AttachDatagramEndpointToSessionMessage(Guid sessionId)
        {
            SessionId = sessionId;
        }
    }
}
