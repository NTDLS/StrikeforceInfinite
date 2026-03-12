using Ae.Engine;
using Ae.MpClientToServerComms;
using NTDLS.Semaphore;
using System.Diagnostics.CodeAnalysis;

namespace Ae.MpServer
{
    internal class EngineManager(ServerInstance mpServerInstance)
    {
        //Dictionary of LobbyId to EngineCore
        private readonly OptimisticCriticalResource<Dictionary<Guid, AeEngine>> _collection = new();

        public AeEngine Create(ManagedLobby lobby)
        {
            var engine = new AeEngine(lobby, mpServerInstance.SharedEngine, AeEngineExecutionMode.ServerHost);

            _collection.Write(o =>
            {
                Console.WriteLine($"Engine created for LobbyId: {lobby.LobbyId}");
                o.Add(lobby.LobbyId, engine);
            });

            return engine;
        }

        public bool TryGet(Guid lobbyId, [NotNullWhen(true)] out AeEngine? engine)
        {
            engine = _collection.Read(o =>
            {
                o.TryGetValue(lobbyId, out var engine);
                return engine;
            });
            return engine != null;
        }

        public AeEngine? Get(Guid lobbyId)
        {
            return _collection.Read(o =>
            {
                o.TryGetValue(lobbyId, out var engine);
                return engine;
            });
        }

        public void Delete(Guid lobbyId)
        {
            _collection.Write(o =>
            {
                if (o.TryGetValue(lobbyId, out var engine))
                {
                    //TODO: Clean up engine resources if needed.

                    engine.ShutdownEngine();

                    Console.WriteLine($"Deleting engine for LobbyId: {lobbyId}");
                    o.Remove(lobbyId);
                }
            });
        }
    }
}
