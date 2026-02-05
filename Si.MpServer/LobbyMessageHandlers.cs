using NTDLS.ReliableMessaging;
using Si.MpLibrary.ReliableMessages;

namespace Si.MpServer
{
    internal class LobbyMessageHandlers(MpServerInstance mpServerInstance) : IRmMessageHandler
    {
        public CreateLobbyQueryReply CreateLobbyQuery(RmContext context, CreateLobbyQuery payload)
        {
            //mpServerInstance.
            return new CreateLobbyQueryReply();
        }
    }
}

