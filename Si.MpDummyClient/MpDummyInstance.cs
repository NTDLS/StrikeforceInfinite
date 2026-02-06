using NTDLS.DatagramMessaging;
using NTDLS.ReliableMessaging;
using Si.MpLibrary;
using Si.MpLibrary.DatagramMessages;
using Si.MpLibrary.ReliableMessages;

namespace Si.MpDummyClient
{
    internal class MpDummyInstance
    {
        private readonly DmClient _dmClient;
        private readonly RmClient _rmClient;

        public MpDummyInstance()
        {
            _dmClient = new DmClient();
            _dmClient.AddHandler(new DatagramMessageHandler(this));
            _dmClient.OnException += (DmContext? context, Exception ex) =>
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

            var udpPort = DmClient.GetRandomUnusedUdpPort();
            Console.WriteLine($"Starting datagram messaging client, listening on port {udpPort}.");
            _dmClient.Listen(udpPort);
            _dmClient.Connect(MpLibraryConstants.DefaultAddress, MpLibraryConstants.DefaultPort);

            Console.WriteLine("MP Dummy Client is running...");

            var session = _rmClient.Query(new StartServerSessionQuery()).EnsureQuerySuccess();
            Console.WriteLine($"Session created: {session.SessionId}");

            var lobby = _rmClient.Query(new CreateLobbyQuery()).EnsureQuerySuccess();
            Console.WriteLine($"Lobby created: {lobby.LobbyId}");

            var attachMessage = new AttachDatagramEndpointToSessionMessage(session.SessionId, lobby.LobbyId);
            _dmClient.Dispatch(attachMessage);
            Console.WriteLine($"Datagram session attached to session: {session.SessionId}");


            _rmClient.Query(new SetSituationQuery(lobby.LobbyId, "SituationDebuggingGalore")).EnsureQuerySuccess();
            Console.WriteLine($"Situation set");

            _rmClient.Query(new StartGameQuery(lobby.LobbyId)).EnsureQuerySuccess();
            Console.WriteLine($"Game started.");

            Console.WriteLine("Press ENTER to stop.");
        }
    }
}
