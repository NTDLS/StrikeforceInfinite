using Si.MpClientToServerComms;

namespace Si.MpHeadlessLobby
{
    internal class MpLobbyHost
        : IDisposable
    {
        //private readonly DmMessenger _dmMessenger;
        private readonly MpCommsManager _commsManager;
        private bool disposedValue;

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

            _commsManager = new MpCommsManager("127.0.0.1", 42719);
        }

        public void Run()
        {
            Console.WriteLine("Waiting on server to init...");
            Thread.Sleep(5000);

            Console.WriteLine("Starting multiplay client...");

            Console.WriteLine("MP Dummy Client is running...");

            var session = _commsManager.StartServerSession();
            Console.WriteLine($"Session created: {session.SessionId}");

            var lobby = _commsManager.CreateLobby("Dire Debugging", 10);
            Console.WriteLine($"Lobby created: {lobby.LobbyId}");

            _commsManager.SetSituation(lobby.LobbyId, "SituationDebuggingGalore");
            Console.WriteLine($"Situation set");

            _commsManager.StartGame(lobby.LobbyId);
            Console.WriteLine($"Game started.");

            Console.WriteLine("Press ENTER to stop.");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _commsManager.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
