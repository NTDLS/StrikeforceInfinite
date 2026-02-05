using NTDLS.DatagramMessaging;
using NTDLS.ReliableMessaging;
using NTDLS.Semaphore;
using Si.MpLibrary;
using System.Threading.Tasks;

namespace Si.MpServer
{
    internal class MpServerInstance
    {
        private readonly DmClient _dmClient;
        private readonly RmServer _rmServer;
        private readonly OptimisticCriticalResource<Dictionary<Guid, MpServerSession>> _sessions = new();

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
            _rmServer.OnDisconnected += _rmServer_OnDisconnected;

            _rmServer.AddHandler(new ReliableMessageHandler(this));
        }

        private void _rmServer_OnDisconnected(RmContext context)
        {
            _sessions.Write(o =>
            {
                if (o.TryGetValue(context.ConnectionId, out var session))
                {
                    //TODO: Clean up session resources if needed.

                    Console.WriteLine($"Session ended with SessionId: {session.SessionId}");
                    o.Remove(context.ConnectionId);
                }
            });
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

        public MpServerSession CreateSession(Guid rmConnectionId)
        {
            var session = new MpServerSession();

            _sessions.Write(o =>
            {
                Console.WriteLine($"Session started with SessionId: {session.SessionId}");
                o.Add(rmConnectionId, session);
            });

            return session;
        }
    }
}
