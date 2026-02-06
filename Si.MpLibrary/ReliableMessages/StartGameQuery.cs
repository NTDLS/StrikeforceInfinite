using NTDLS.ReliableMessaging;

namespace Si.MpLibrary.ReliableMessages
{
    public class StartGameQuery
        : IRmQuery<StartGameQueryReply>
    {
        public Guid LobbyId { get; set; }

        public StartGameQuery()
        {
        }

        public StartGameQuery(Guid lobbyId)
        {
            LobbyId = lobbyId;
        }
    }

    public class StartGameQueryReply
        : IRmQueryReply, IMultiPlayQueryReply
    {
        public string? ErrorMessage { get; set; }

        public StartGameQueryReply()
        {
        }

        public StartGameQueryReply(Exception ex)
        {
            ErrorMessage = ex.GetBaseException().Message;
        }
    }
}
