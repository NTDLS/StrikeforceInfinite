using NTDLS.DatagramMessaging;
using NTDLS.Semaphore;
using Si.MpComms;
using System.Diagnostics.CodeAnalysis;

namespace Si.MpServer
{
    internal class LobbyManager(ServerInstance mpServerInstance)
    {
        //Dictionary of LobbyId to Lobby
        private readonly OptimisticCriticalResource<Dictionary<Guid, Lobby>> _collection = new();

        public Lobby Create(Session session, DmMessenger dmMessenger)
        {
            var lobby = new Lobby(session, dmMessenger);

            _collection.Write(o =>
            {
                Console.WriteLine($"Lobby created LobbyId: {lobby.LobbyId}");
                o.Add(lobby.LobbyId, lobby);
            });

            return lobby;
        }

        public bool TryGet(Guid lobbyId, [NotNullWhen(true)] out Lobby? lobby)
        {
            lobby = _collection.Read(o =>
            {
                o.TryGetValue(lobbyId, out var lobby);
                return lobby;
            });
            return lobby != null;
        }

        public Lobby? Get(Guid lobbyId)
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
