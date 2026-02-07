using NTDLS.DatagramMessaging;
using NTDLS.ReliableMessaging;
using Si.MpLibrary;
using Si.MpLibrary.DatagramMessages;
using Si.MpLibrary.ReliableMessages;
using System.Net.Sockets;

namespace Si.MpDummyClient
{
    internal class MpDummyInstance
    {
        private readonly DmMessenger _dmMessenger;
        private readonly RmClient _rmClient;

        public MpDummyInstance()
        {
            var udpPort = DmMessenger.GetRandomUnusedUdpPort();

            _dmMessenger = new DmMessenger(udpPort);
            _dmMessenger.AddHandler(new DatagramMessageHandler(this));
            _dmMessenger.OnException += (DmContext? context, Exception ex) =>
            {
                Console.WriteLine($"[Client - DM Client] Exception: {ex.GetBaseException().Message}");
            };

            _rmClient = new RmClient();
            _rmClient.OnException += (RmContext? context, Exception ex, IRmPayload? payload) =>
            {
                Console.WriteLine($"[Client - RM Client] Exception: {ex.GetBaseException().Message}");
            };
        }

        public void Run()
        {
            Console.WriteLine("Waiting on server to init...");
            Thread.Sleep(5000);

            Console.WriteLine("Starting multiplay client...");

            Console.WriteLine("Starting reliable messaging client.");
            _rmClient.Connect(MpLibraryConstants.DefaultAddress, MpLibraryConstants.DefaultPort);

            Console.WriteLine($"Datagram messaging client listening on port {_dmMessenger.ListenPort}.");
            //_dmMessenger.Connect(MpLibraryConstants.DefaultAddress, MpLibraryConstants.DefaultPort);

            var serverEndpointCtx = _dmMessenger.GetEndpointContext(MpLibraryConstants.DefaultAddress, MpLibraryConstants.DefaultPort);

            Console.WriteLine("MP Dummy Client is running...");

            var session = _rmClient.Query(new StartServerSessionQuery()).EnsureQuerySuccess();
            Console.WriteLine($"Session created: {session.SessionId}");

            var lobby = _rmClient.Query(new CreateLobbyQuery()).EnsureQuerySuccess();
            Console.WriteLine($"Lobby created: {lobby.LobbyId}");

            var attachMessage = new AttachDatagramEndpointToSessionMessage(session.SessionId, lobby.LobbyId);
            _dmMessenger.Dispatch(attachMessage, serverEndpointCtx);
            Console.WriteLine($"Datagram session attached to session: {session.SessionId}");

            _rmClient.Query(new SetSituationQuery(lobby.LobbyId, "SituationDebuggingGalore")).EnsureQuerySuccess();
            Console.WriteLine($"Situation set");

            _rmClient.Query(new StartGameQuery(lobby.LobbyId)).EnsureQuerySuccess();
            Console.WriteLine($"Game started.");

            Console.WriteLine("Press ENTER to stop.");
        }
    }
}
