using NTDLS.DatagramMessaging;
using NTDLS.ReliableMessaging;
using Si.MpLibrary;
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
            Console.WriteLine("Starting multiplay client...");

            Console.WriteLine("Starting reliable messaging client.");
            _rmClient.Connect(MpLibraryConstants.DefaultAddress, MpLibraryConstants.DefaultPort);

            Console.WriteLine("Starting datagram messaging client.");
            _dmClient.Listen(DmClient.GetRandomUnusedUdpPort());
            _dmClient.Connect(MpLibraryConstants.DefaultAddress, MpLibraryConstants.DefaultPort);

            Console.WriteLine("MP Dummy Client is running...");


            _rmClient.Query(new StartServerSessionQuery()).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine($"CreateLobbyQuery failed: {task.Exception?.GetBaseException().Message}");
                    return;
                }

                Console.WriteLine($"Session started with SessionId: {task.Result.SessionId}");
            });

            Console.WriteLine("Press ENTER to stop.");
        }
    }
}
