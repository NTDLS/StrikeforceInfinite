using NTDLS.ReliableMessaging;

namespace Si.MpCommsMessages.ReliableMessages
{
    public class StartServerSessionQuery
        : IRmQuery<StartServerSessionQueryReply>
    {
    }

    public class StartServerSessionQueryReply
        : IRmQueryReply, IMultiPlayQueryReply
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
