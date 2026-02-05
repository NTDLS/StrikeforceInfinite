using NTDLS.DatagramMessaging;
using NTDLS.ReliableMessaging;
using Si.MpLibrary;

namespace Si.MpServer
{
    internal class ServerInstance
    {
        private readonly DmClient _dmClient;
        private readonly RmServer _rmServer;

        internal SessionManager Sessions { get; set; }
        internal LobbyManager Lobbies { get; set; }

        public ServerInstance()
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
            _rmServer.OnDisconnected += _rmServer_OnDisconnected;

            _rmServer.AddHandler(new ReliableMessageHandler(this));

            Sessions = new SessionManager(this);
            Lobbies = new LobbyManager(this);
        }

        private void _rmServer_OnDisconnected(RmContext context)
        {
            Sessions.Delete(context.ConnectionId);
        }

        public void Run()
        {
            Console.WriteLine("Starting multiplay server...");

            Console.WriteLine("Starting reliable messaging server.");
            _rmServer.Start(MpLibraryConstants.DefaultPort);

            Console.WriteLine("Starting datagram messaging client.");
            _dmClient.Listen(MpLibraryConstants.DefaultPort);

            Console.WriteLine("MP Server is running...");

            Console.WriteLine("Press ENTER to stop.");
        }

    }
}
