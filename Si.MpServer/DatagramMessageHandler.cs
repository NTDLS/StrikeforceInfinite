using NTDLS.DatagramMessaging;
using Si.MpCommsMessages.DatagramMessages;

namespace Si.MpServer
{
    internal class DatagramMessageHandler(ServerInstance mpServerInstance)
        : IDmDatagramHandler
    {
        public void AttachDatagramEndpointToSessionMessage(DmContext context, AttachDatagramEndpointToSessionMessage payload)
        {
            try
            {
                if (!mpServerInstance.Sessions.TryGetBySessionId(payload.SessionId, out var session))
                {
                    throw new Exception($"Session not found for SessionId {payload.SessionId}.");
                }

                session.AttachDatagramEndpoint(context.Endpoint);

                /*
                if (!mpServerInstance.Lobbies.TryGet(payload.LobbyId, out var lobby))
                {
                    throw new Exception($"Lobby not found for LobbyId {payload.LobbyId}.");
                }
                */

                // Attach the datagram endpoint to the session
            }
            catch (Exception ex)
            {
            }
        }
    }
}
