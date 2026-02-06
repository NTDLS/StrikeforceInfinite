using NTDLS.ReliableMessaging;

namespace Si.MpLibrary.ReliableMessages
{
    public class StartServerSessionQuery
        : IRmQuery<StartServerSessionQueryReply>
    {
    }

    public class StartServerSessionQueryReply
        : IRmQueryReply
    {
        public Guid SessionId { get; set; }
        public string? ErrorMessage { get; set; }

        public StartServerSessionQueryReply()
        {
        }

        public StartServerSessionQueryReply(Exception ex)
        {
            ErrorMessage = ex.GetBaseException().Message;
        }

        public StartServerSessionQueryReply(Guid sessionId)
        {
            SessionId = sessionId;
        }
    }
}
