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

        public StartServerSessionQueryReply()
        {
        }

        public StartServerSessionQueryReply(Guid sessionId)
        {
            SessionId = sessionId;
        }
    }
}
