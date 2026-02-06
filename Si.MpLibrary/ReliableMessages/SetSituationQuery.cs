using NTDLS.ReliableMessaging;

namespace Si.MpLibrary.ReliableMessages
{
    public class SetSituationQuery
        : IRmQuery<SetSituationQueryReply>
    {
        public Guid LobbyId { get; set; }
        public string SituationName { get; set; } = string.Empty;

        public SetSituationQuery()
        {
        }

        public SetSituationQuery(Guid lobbyId, string situationName)
        {
            LobbyId = lobbyId;
            SituationName = situationName;
        }
    }

    public class SetSituationQueryReply
        : IRmQueryReply, IMultiPlayQueryReply
    {
        public string? ErrorMessage { get; set; }

        public SetSituationQueryReply()
        {
        }

        public SetSituationQueryReply(Exception ex)
        {
            ErrorMessage = ex.GetBaseException().Message;
        }
    }
}
