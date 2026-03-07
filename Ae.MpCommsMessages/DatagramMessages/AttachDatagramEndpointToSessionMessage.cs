using NTDLS.DatagramMessaging;

namespace Ae.MpCommsMessages.DatagramMessages
{
    public class AttachDatagramEndpointToSessionMessage
        : IDmDatagram
    {
        public Guid SessionId { get; set; }

        public AttachDatagramEndpointToSessionMessage()
        {
        }

        public AttachDatagramEndpointToSessionMessage(Guid sessionId)
        {
            SessionId = sessionId;
        }
    }
}
