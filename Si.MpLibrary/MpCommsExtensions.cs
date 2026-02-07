using NTDLS.DatagramMessaging;
using NTDLS.ReliableMessaging;
using Si.MpLibrary.DatagramMessages;
using Si.MpLibrary.ReliableMessages;

namespace Si.MpLibrary
{
    /// <summary>
    /// MpComms: MultiPlayer Communications.
    /// 
    /// Used for easily calling queries from the client. These are just thin wrappers around RmClient.Query
    /// that ensure the query was successful and throw an exception if not.
    /// </summary>
    public static class MpCommsExtensions
    {
        public static SetSituationQueryReply SetSituation(this RmClient rmClient, Guid LobbyId, string situationName)
            => rmClient.Query(new SetSituationQuery(LobbyId, situationName)).EnsureQuerySuccess();

        public static StartGameQueryReply StartGame(this RmClient rmClient, Guid LobbyId)
            => rmClient.Query(new StartGameQuery(LobbyId)).EnsureQuerySuccess();

        public static StartServerSessionQueryReply StartServerSession(this RmClient rmClient)
            => rmClient.Query(new StartServerSessionQuery()).EnsureQuerySuccess();

        public static CreateLobbyQueryReply CreateLobby(this RmClient rmClient, string lobbyName, int maxPlayers)
            => rmClient.Query(new CreateLobbyQuery(lobbyName, maxPlayers)).EnsureQuerySuccess();

        public static void AttachDatagramEndpointToSession(this DmMessenger dmMessenger, DmContext dmContext, Guid sessionId)
            => dmMessenger.Dispatch(new AttachDatagramEndpointToSessionMessage(sessionId), dmContext);


        //var attachMessage = new AttachDatagramEndpointToSessionMessage(session.SessionId, lobby.LobbyId);
        //_dmMessenger.Dispatch(attachMessage, serverEndpointCtx);
        //Console.WriteLine($"Datagram session attached to session: {session.SessionId}");
    }
}
