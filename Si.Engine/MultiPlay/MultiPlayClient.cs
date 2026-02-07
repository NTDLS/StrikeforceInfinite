using NTDLS.DatagramMessaging;
using NTDLS.ReliableMessaging;
using Si.MpLibrary;
using System;

namespace Si.Engine.MultiPlay
{
    internal class MultiPlayClient
    {
        private readonly DmMessenger _dmMessenger;
        private readonly RmClient _rmClient;
        private readonly EngineCore _engineCore;

        public MultiPlayClient(EngineCore engineCore)
        {
            _engineCore = engineCore;

            Console.WriteLine("Starting multiplay client...");

            var udpPort = DmMessenger.GetRandomUnusedUdpPort();

            _dmMessenger = new DmMessenger(udpPort);
            _dmMessenger.AddHandler(new DatagramMessageHandler(this));
            _dmMessenger.OnException += (DmContext? context, Exception ex) =>
            {
                Console.WriteLine($"[Client - DM Client] Exception: {ex.GetBaseException().Message}");
            };
            Console.WriteLine($"Datagram messaging client listening on port {_dmMessenger.ListenPort}.");

            _rmClient = new RmClient();
            _rmClient.OnException += (RmContext? context, Exception ex, IRmPayload? payload) =>
            {
                Console.WriteLine($"[Client - RM Client] Exception: {ex.GetBaseException().Message}");
            };

            Console.WriteLine("Starting reliable messaging client.");
            _rmClient.Connect(MpLibraryConstants.DefaultAddress, MpLibraryConstants.DefaultPort);

            var serverEndpointCtx = _dmMessenger.GetEndpointContext(MpLibraryConstants.DefaultAddress, MpLibraryConstants.DefaultPort);

            var session = _rmClient.StartServerSession();
            Console.WriteLine($"Session created: {session.SessionId}");

            var lobby = _rmClient.CreateLobby("LobbyName", 4);
            Console.WriteLine($"Lobby created: {lobby.LobbyId}");

            _dmMessenger.AttachDatagramEndpointToSession(serverEndpointCtx, session.SessionId);
            Console.WriteLine($"Datagram session attached to session: {session.SessionId}");

            _rmClient.SetSituation(lobby.LobbyId, "SituationDebuggingGalore");
            Console.WriteLine($"Situation set");

            _rmClient.StartGame(lobby.LobbyId);
            Console.WriteLine($"Game started.");
        }

        public void Shutdown()
        {
            _rmClient.Disconnect();
            _dmMessenger.Dispose();
        }
    }
}
