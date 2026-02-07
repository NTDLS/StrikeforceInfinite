using Si.MpComms;
using Si.MpLibrary;
using System;

namespace Si.Engine.MultiPlay
{
    internal class MultiPlayClient
    {
        private readonly MpCommsManager _commsManager;
        private readonly EngineCore _engineCore;

        public MultiPlayClient(EngineCore engineCore)
        {
            _engineCore = engineCore;

            Console.WriteLine("Starting multiplay client...");

            _commsManager = new MpCommsManager(MpLibraryConstants.DefaultAddress, MpLibraryConstants.DefaultPort);

            _commsManager.AddHandler(new DatagramMessageHandler(this));

            var session = _commsManager.StartServerSession();
            Console.WriteLine($"Session created: {session.SessionId}");

            var lobby = _commsManager.CreateLobby("LobbyName", 4);
            Console.WriteLine($"Lobby created: {lobby.LobbyId}");

            _commsManager.AttachDatagramEndpointToSession(session.SessionId);
            Console.WriteLine($"Datagram session attached to session: {session.SessionId}");

            _commsManager.SetSituation(lobby.LobbyId, "SituationDebuggingGalore");
            Console.WriteLine($"Situation set");

            _commsManager.StartGame(lobby.LobbyId);
            Console.WriteLine($"Game started.");
        }

        public void Shutdown()
        {
            _commsManager.Dispose();
        }
    }
}
