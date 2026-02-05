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
            _rmServer = new RmServer();
        }

        public void Run()
        {
            Console.WriteLine("Starting reliable messaging.");
            _rmServer.Start(MpLibraryConstants.DefaultPort);

            Console.WriteLine("Starting datagram messaging.");
            _dmClient.Listen(MpLibraryConstants.DefaultPort);

            Console.WriteLine("MP Server is running...");

            Console.WriteLine("Press ENTER to stop the server.");
        }
    }
}
