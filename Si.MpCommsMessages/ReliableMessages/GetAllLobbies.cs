using NTDLS.ReliableMessaging;

namespace Si.MpCommsMessages.ReliableMessages
{
    public class GetAllLobbiesQuery
        : IRmQuery<GetAllLobbiesQueryReply>
    {
        public GetAllLobbiesQuery()
        {
        }
    }

    public class GetAllLobbiesQueryReply
        : IRmQueryReply, IMultiPlayQueryReply
    {
        public Guid LobbyId { get; set; }
        public string? ErrorMessage { get; set; }

        public GetAllLobbiesQueryReply()
        {
        }

        public GetAllLobbiesQueryReply(Exception ex)
        {
            ErrorMessage = ex.GetBaseException().Message;
        }

        public GetAllLobbiesQueryReply(Guid lobbyId)
        {
            LobbyId = lobbyId;
        }
    }
}
