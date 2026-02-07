using NTDLS.ReliableMessaging;

namespace Si.MpCommsMessages.ReliableMessages
{
    public class CreateLobbyQuery
        : IRmQuery<CreateLobbyQueryReply>
    {
        public string LobbyName { get; set; } = string.Empty;
        public int MaxPlayers { get; set; }

        public CreateLobbyQuery()
        {
        }

        public CreateLobbyQuery(string lobbyName, int maxPlayers)
        {
            LobbyName = lobbyName;
            MaxPlayers = maxPlayers;
        }
    }

    public class CreateLobbyQueryReply
        : IRmQueryReply, IMultiPlayQueryReply
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
