using NTDLS.DatagramMessaging;

namespace Si.MpLibrary.DatagramMessages
{
    public class AttachDatagramEndpointToSessionMessage
        : IDmDatagram
    {
        public Guid SessionId { get; set; }
        public Guid LobbyId { get; set; }

        public AttachDatagramEndpointToSessionMessage()
        {
        }

        public AttachDatagramEndpointToSessionMessage(Guid sessionId, Guid lobbyId)
        {
            SessionId = sessionId;
            LobbyId = lobbyId;
        }
    }
}
