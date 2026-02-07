using NTDLS.ReliableMessaging;
using Si.MpLibrary;
using Si.MpLibrary.ReliableMessages;

namespace Si.MpHeadlessLobby
{
    internal class MpLobbyHost
    {
        //private readonly DmMessenger _dmMessenger;
        private readonly RmClient _rmClient;

        public MpLobbyHost()
        {
            /*
            var udpPort = DmMessenger.GetRandomUnusedUdpPort();

            _dmMessenger = new DmMessenger(udpPort);
            _dmMessenger.AddHandler(new DatagramMessageHandler(this));
            _dmMessenger.OnException += (DmContext? context, Exception ex) =>
            {
                Console.WriteLine($"[Client - DM Client] Exception: {ex.GetBaseException().Message}");
            };
            */

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

            //Console.WriteLine($"Datagram messaging client listening on port {_dmMessenger.ListenPort}.");
            //_dmMessenger.Connect(MpLibraryConstants.DefaultAddress, MpLibraryConstants.DefaultPort);

            //var serverEndpointCtx = _dmMessenger.GetEndpointContext(MpLibraryConstants.DefaultAddress, MpLibraryConstants.DefaultPort);

            Console.WriteLine("MP Dummy Client is running...");

            var session = _rmClient.StartServerSession();
            Console.WriteLine($"Session created: {session.SessionId}");

            var lobby = _rmClient.CreateLobby("Dire Debugging", 10);
            Console.WriteLine($"Lobby created: {lobby.LobbyId}");

            //_dmMessenger.AttachDatagramEndpointToSession(serverEndpointCtx, session.SessionId);
            //Console.WriteLine($"Datagram session attached to session: {session.SessionId}");

            _rmClient.SetSituation(lobby.LobbyId, "SituationDebuggingGalore");
            Console.WriteLine($"Situation set");

            _rmClient.Query(new StartGameQuery(lobby.LobbyId)).EnsureQuerySuccess();
            Console.WriteLine($"Game started.");

            Console.WriteLine("Press ENTER to stop.");
        }
    }
}
