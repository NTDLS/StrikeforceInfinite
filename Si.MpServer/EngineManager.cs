using NTDLS.Semaphore;
using Si.Engine;
using Si.MpLibrary;
using System.Diagnostics.CodeAnalysis;
using static Si.Library.SiConstants;

namespace Si.MpServer
{
    internal class EngineManager(ServerInstance mpServerInstance)
    {
        //Dictionary of LobbyId to EngineCore
        private readonly OptimisticCriticalResource<Dictionary<Guid, EngineCore>> _collection = new();

        public EngineCore Create(Lobby lobby)
        {
            var engine = new EngineCore(lobby, mpServerInstance.SharedEngine, SiEngineExecutionMode.ServerHost);

            _collection.Write(o =>
            {
                Console.WriteLine($"Engine created for LobbyId: {lobby.LobbyId}");
                o.Add(lobby.LobbyId, engine);
            });

            return engine;
        }

        public bool TryGet(Guid lobbyId, [NotNullWhen(true)] out EngineCore? engine)
        {
            engine = _collection.Read(o =>
            {
                o.TryGetValue(lobbyId, out var engine);
                return engine;
            });
            return engine != null;
        }

        public EngineCore? Get(Guid lobbyId)
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
