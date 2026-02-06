using NTDLS.Semaphore;
using Si.Engine;
using Si.MpLibrary;

namespace Si.MpServer
{
    internal class LobbyManager(ServerInstance mpServerInstance)
    {
        //Dictionary of LobbyId to Lobby
        private readonly OptimisticCriticalResource<Dictionary<Guid, Lobby>> _collection = new();

        public Lobby Create(Session session)
        {
            var lobby = new Lobby(session);

            _collection.Write(o =>
            {
                Console.WriteLine($"Lobby created LobbyId: {lobby.LobbyId}");
                o.Add(lobby.LobbyId, lobby);
            });

            return lobby;
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
