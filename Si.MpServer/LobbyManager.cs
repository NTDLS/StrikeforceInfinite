using NTDLS.DatagramMessaging;
using NTDLS.Semaphore;
using Si.MpClientToServerComms;
using System.Diagnostics.CodeAnalysis;

namespace Si.MpServer
{
    internal class LobbyManager(ServerInstance mpServerInstance)
    {
        //Dictionary of LobbyId to Lobby
        private readonly OptimisticCriticalResource<Dictionary<Guid, ManagedLobby>> _collection = new();

        public ManagedLobby Create(ManagedSession session, DmMessenger dmMessenger)
        {
            var lobby = new ManagedLobby(session, dmMessenger);

            _collection.Write(o =>
            {
                Console.WriteLine($"Lobby created LobbyId: {lobby.LobbyId}");
                o.Add(lobby.LobbyId, lobby);
            });

            return lobby;
        }

        public bool TryGet(Guid lobbyId, [NotNullWhen(true)] out ManagedLobby? lobby)
        {
            lobby = _collection.Read(o =>
            {
                o.TryGetValue(lobbyId, out var lobby);
                return lobby;
            });
            return lobby != null;
        }

        public ManagedLobby? Get(Guid lobbyId)
        {
            return _collection.Read(o =>
            {
                o.TryGetValue(lobbyId, out var lobby);
                return lobby;
            });
        }

        public void Delete(Guid lobbyId)
        {
            _collection.Write(o =>
            {
                if (o.TryGetValue(lobbyId, out var lobby))
                {
                    //TODO: Clean up lobby resources if needed.

                    Console.WriteLine($"Deleting lobby with LobbyId: {lobby.LobbyId}");
                    o.Remove(lobby.LobbyId);
                }
            });
        }
    }
}
