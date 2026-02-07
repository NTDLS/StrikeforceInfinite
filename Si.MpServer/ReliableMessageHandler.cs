using NTDLS.ReliableMessaging;
using Si.MpLibrary.ReliableMessages;

namespace Si.MpServer
{
    internal class ReliableMessageHandler(ServerInstance mpServerInstance)
        : IRmMessageHandler
    {
        public CreateLobbyQueryReply CreateLobbyQuery(RmContext context, CreateLobbyQuery payload)
        {
            try
            {
                if (!mpServerInstance.Sessions.TryGetByConnectionId(context.ConnectionId, out var session))
                {
                    throw new Exception($"Session not found for ConnectionId {context.ConnectionId}.");
                }

                var lobby = mpServerInstance.Lobbies.Create(session, mpServerInstance.DmMessenger);

                var engine = mpServerInstance.Engines.Create(lobby)
                    ?? throw new Exception("Failed to create game for lobby.");

                engine.StartEngine();

                return new CreateLobbyQueryReply(lobby.LobbyId);

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
            catch (Exception ex)
            {
                return new StartServerSessionQueryReply(ex);
            }
        }

        public StartGameQueryReply StartGameQuery(RmContext context, StartGameQuery payload)
        {
            try
            {
                if (!mpServerInstance.Lobbies.TryGet(payload.LobbyId, out var lobby))
                {
                    throw new Exception($"Lobby not found for LobbyId {payload.LobbyId}.");
                }

                if (!mpServerInstance.Engines.TryGet(payload.LobbyId, out var engine))
                {
                    throw new Exception($"Engine not found for LobbyId {payload.LobbyId}.");
                }

                engine.StartGame();

                return new StartGameQueryReply();
            }
            catch (Exception ex)
            {
                return new StartGameQueryReply(ex);
            }
        }

        public SetSituationQueryReply SetSituationQuery(RmContext context, SetSituationQuery payload)
        {
            try
            {
                if (!mpServerInstance.Lobbies.TryGet(payload.LobbyId, out var lobby))
                {
                    throw new Exception($"Lobby not found for LobbyId {payload.LobbyId}.");
                }

                if (!mpServerInstance.Engines.TryGet(payload.LobbyId, out var engine))
                {
                    throw new Exception($"Engine not found for LobbyId {payload.LobbyId}.");
                }

                engine.Situations.Select(payload.SituationName);

                return new SetSituationQueryReply();
            }
            catch (Exception ex)
            {
                return new SetSituationQueryReply(ex);
            }
        }
    }
}
