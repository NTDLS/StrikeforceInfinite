using NTDLS.DatagramMessaging;
using NTDLS.ReliableMessaging;
using Si.MpLibrary;

namespace Si.MpServer
{
    internal class MpServerInstance
    {
        private readonly DmClient _dmClient;
        private readonly RmServer _rmServer;

        public MpServerInstance()
        {
            _dmClient = new DmClient();
            _dmClient.OnException += (DmContext? context, Exception ex) =>
            {
                Console.WriteLine($"[Server - DM Client] Exception: {ex.GetBaseException().Message}");
            };

            _rmServer = new RmServer();
            _rmServer.OnException += (RmContext? context, Exception ex, IRmPayload? payload) =>
            {
                Console.WriteLine($"[Server - RM Server] Exception: {ex.GetBaseException().Message}");
            };

            _rmServer.AddHandler(new LobbyMessageHandlers(this));
        }

        public void Run()
        {
            Console.WriteLine("Starting reliable messaging server.");
            _rmServer.Start(MpLibraryConstants.DefaultPort);

            Console.WriteLine("Starting datagram messaging client.");
            _dmClient.Listen(MpLibraryConstants.DefaultPort);

            Console.WriteLine("MP Server is running...");

            Console.WriteLine("Press ENTER to stop.");
        }
    }
}
