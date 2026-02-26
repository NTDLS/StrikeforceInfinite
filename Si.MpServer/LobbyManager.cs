using NTDLS.DatagramMessaging;
using NTDLS.Semaphore;
using Si.MpClientToServerComms;
using Si.MpCommsMessages.Models;
using System.Diagnostics.CodeAnalysis;

namespace Si.MpServer
{
    internal class LobbyManager(ServerInstance mpServerInstance)
    {
        //Dictionary of LobbyId to Lobby
        private readonly OptimisticCriticalResource<Dictionary<Guid, ManagedLobby>> _collection = new();

        public Lobby[] GetManagedLobbiesPaged(int pageNumber, int pageSize, out int totalCountOfLobbies)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than 0.");
            }

            totalCountOfLobbies = _collection.Read(o => o.Count);

            var managedLobbies = _collection.Read(o =>
            {
                return o.Values.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();
            });

            List<Lobby> lobbies = new();

            foreach (var managedLobby in managedLobbies)
            {
                lobbies.Add(new Lobby
                {
                    LobbyId = managedLobby.LobbyId,
                    Name = managedLobby.Name,
                    MaxPlayers = managedLobby.MaxPlayers,
                    CurrentPlayers = managedLobby.Sessions.Read(s => s.Count)
                });
            }

            return lobbies.ToArray();
        }

        public ManagedLobby Create(ManagedSession session, DmMessenger dmMessenger)
        {
            var managedLobby = new ManagedLobby(session, dmMessenger);

            _collection.Write(o =>
            {
                Console.WriteLine($"Lobby created LobbyId: {managedLobby.LobbyId}");
                o.Add(managedLobby.LobbyId, managedLobby);
            });

            return managedLobby;
        }

        public bool TryGet(Guid lobbyId, [NotNullWhen(true)] out ManagedLobby? managedLobby)
        {
            managedLobby = _collection.Read(o =>
            {
                o.TryGetValue(lobbyId, out var managedLobby);
                return managedLobby;
            });
            return managedLobby != null;
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
                if (o.TryGetValue(lobbyId, out var managedLobby))
                {
                    //TODO: Clean up lobby resources if needed.

                    Console.WriteLine($"Deleting lobby with LobbyId: {managedLobby.LobbyId}");
                    o.Remove(managedLobby.LobbyId);
                }
            });
        }
    }
}
