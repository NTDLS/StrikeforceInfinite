using NTDLS.ReliableMessaging;
using Si.MpLibrary.ReliableMessages;

namespace Si.MpServer
{
    internal class ReliableMessageHandler(MpServerInstance mpServerInstance) : IRmMessageHandler
    {
        public CreateLobbyQueryReply CreateLobbyQuery(RmContext context, CreateLobbyQuery payload)
        {
            return new CreateLobbyQueryReply();
        }

        public StartServerSessionQueryReply StartServerSessionQuery(RmContext context, StartServerSessionQuery payload)
        {
            var sessionId = mpServerInstance.CreateSession(context.ConnectionId).SessionId;
            return new StartServerSessionQueryReply(sessionId);
        }
    }
}
