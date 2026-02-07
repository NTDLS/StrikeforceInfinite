using NTDLS.DatagramMessaging;
using NTDLS.ReliableMessaging;
using Si.Engine;
using Si.MpServer.MessageHandlers;
using static Si.Library.SiConstants;

namespace Si.MpServer
{
    internal class ServerInstance
    {
        public DmMessenger DmMessenger { get; private set; }
        public RmServer RmServer { get; private set; }
        internal SessionManager Sessions { get; private set; }
        internal LobbyManager Lobbies { get; private set; }
        internal EngineManager Engines { get; private set; }
        internal EngineCore SharedEngine { get; private set; } = new(SiEngineExecutionMode.SharedEngineContent);

        private readonly int _listenPort = 42719;

        public ServerInstance(int listenPort)
        {
            _listenPort = listenPort;

            DmMessenger = new DmMessenger(listenPort);

            DmMessenger.AddHandler(new DatagramMessageHandler(this));
            DmMessenger.OnException += (DmContext? context, Exception ex) =>
            {
                Console.WriteLine($"[Server - DM Client] Exception: {ex.GetBaseException().Message}");
            };

            RmServer = new RmServer();
            RmServer.OnException += (RmContext? context, Exception ex, IRmPayload? payload) =>
            {
                Console.WriteLine($"[Server - RM Server] Exception: {ex.GetBaseException().Message}");
            };
            RmServer.OnDisconnected += _rmServer_OnDisconnected;
            RmServer.AddHandler(new ReliableMessageHandler(this));

            Sessions = new SessionManager(this);
            Lobbies = new LobbyManager(this);
            Engines = new EngineManager(this);
        }

        private void _rmServer_OnDisconnected(RmContext context)
        {
            Sessions.Delete(context.ConnectionId);
        }

        public void Run()
        {
            Console.WriteLine("Starting multiplay server...");

            Console.WriteLine("Starting shared engine.");
            SharedEngine.StartEngine();

            Console.WriteLine("Starting reliable messaging server.");
            RmServer.Start(_listenPort);

            Console.WriteLine($"Datagram messaging client listening on port {DmMessenger.ListenPort}.");

            Console.WriteLine("MP Server is running...");

            Console.WriteLine("Press ENTER to stop.");
        }
    }
}
