using NTDLS.DatagramMessaging;
using NTDLS.ReliableMessaging;
using Si.MpLibrary;
using Si.MpLibrary.ReliableMessages;
using System.Globalization;
using System.Threading.Tasks;

namespace Si.MpDummyClient
{
    internal class MpDummyInstance
    {
        private readonly DmClient _dmClient;
        private readonly RmClient _rmClient;

        public MpDummyInstance()
        {
            _dmClient = new DmClient();
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

            Console.WriteLine("Starting datagram messaging client.");
            _dmClient.Listen(DmClient.GetRandomUnusedUdpPort());
            _dmClient.Connect(MpLibraryConstants.DefaultAddress, MpLibraryConstants.DefaultPort);

            Console.WriteLine("MP Dummy Client is running...");

            var session = _rmClient.Query(new StartServerSessionQuery()).EnsureQuerySuccess();
            Console.WriteLine($"Session created: {session.SessionId}");

            var lobby = _rmClient.Query(new CreateLobbyQuery()).EnsureQuerySuccess();
            Console.WriteLine($"Lobby created: {lobby.LobbyId}");

            _rmClient.Query(new SetSituationQuery(lobby.LobbyId, "SituationDebuggingGalore")).EnsureQuerySuccess();
            Console.WriteLine($"Situation set");

            _rmClient.Query(new StartGameQuery(lobby.LobbyId)).EnsureQuerySuccess();
            Console.WriteLine($"Game started.");

            Console.WriteLine("Press ENTER to stop.");
        }
    }
}
