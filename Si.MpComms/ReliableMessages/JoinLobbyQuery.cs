using NTDLS.ReliableMessaging;

namespace Si.MpComms.ReliableMessages
{
    public class JoinLobbyQuery
        : IRmQuery<JoinLobbyQueryReply>
    {
        public Guid LobbyId { get; set; }

        public JoinLobbyQuery()
        {
        }

        public JoinLobbyQuery(Guid lobbyId)
        {
            LobbyId = lobbyId;
        }
    }

    public class JoinLobbyQueryReply
        : IRmQueryReply, IMultiPlayQueryReply
    {
        public string? ErrorMessage { get; set; }

        public JoinLobbyQueryReply()
        {
        }

        public JoinLobbyQueryReply(Exception ex)
        {
            ErrorMessage = ex.GetBaseException().Message;
        }
    }
}
