using NTDLS.ReliableMessaging;
using Si.MpLibrary.ReliableMessages;

namespace Si.MpServer
{
    internal class ReliableMessageHandler(ServerInstance mpServerInstance) : IRmMessageHandler
    {
        public CreateLobbyQueryReply CreateLobbyQuery(RmContext context, CreateLobbyQuery payload)
        {
            try
            {
                if (mpServerInstance.Sessions.TryGet(context.ConnectionId, out var session))
                {
                    var lobby = mpServerInstance.Lobbies.Create(session);

                    var engine = mpServerInstance.Engines.Create(lobby)
                        ?? throw new Exception("Failed to create game for lobby.");

                    engine.StartEngine();

                    return new CreateLobbyQueryReply(lobby.LobbyId);
                }

                throw new Exception("Failed to create lobby.");
            }
            catch (Exception ex)
            {
                return new CreateLobbyQueryReply(ex);
            }
        }

        public StartServerSessionQueryReply StartServerSessionQuery(RmContext context, StartServerSessionQuery payload)
        {
            try
            {
                var sessionId = mpServerInstance.Sessions.Create(context.ConnectionId).SessionId;
                return new StartServerSessionQueryReply(sessionId);
            }
            catch(Exception ex)
            {
                return new StartServerSessionQueryReply(ex);
            }
        }
    }
}
