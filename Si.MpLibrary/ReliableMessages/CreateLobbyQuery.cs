using NTDLS.ReliableMessaging;

namespace Si.MpLibrary.ReliableMessages
{
    public class CreateLobbyQuery
        : IRmQuery<CreateLobbyQueryReply>
    {
    }

    public class CreateLobbyQueryReply
        : IRmQueryReply
    {
        public Guid LobbyId { get; set; }
        public string? ErrorMessage { get; set; }

        public CreateLobbyQueryReply()
        {
        }

        public CreateLobbyQueryReply(Exception ex)
        {
            ErrorMessage = ex.GetBaseException().Message;
        }

        public CreateLobbyQueryReply(Guid lobbyId)
        {
            LobbyId = lobbyId;
        }
    }
}
