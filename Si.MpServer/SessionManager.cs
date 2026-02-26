using NTDLS.Semaphore;
using Si.MpClientToServerComms;
using System.Diagnostics.CodeAnalysis;

namespace Si.MpServer
{
    internal class SessionManager(ServerInstance mpServerInstance)
    {
        //Dictionary of RmConnectionId to Session
        private readonly OptimisticCriticalResource<Dictionary<Guid, ManagedSession>> _collection = new();

        public ManagedSession Create(Guid rmConnectionId)
        {
            var session = new ManagedSession(rmConnectionId);

            _collection.Write(o =>
            {
                Console.WriteLine($"Session started with SessionId: {session.SessionId}");
                o.Add(rmConnectionId, session);
            });

            return session;
        }

        public bool TryGetBySessionId(Guid sessionId, [NotNullWhen(true)] out ManagedSession? session)
        {
            session = _collection.Read(o => o.FirstOrDefault(kv => kv.Value.SessionId == sessionId).Value);
            return session != default;
        }

        public ManagedSession? GetBySessionId(Guid sessionId)
        {
            var session = _collection.Read(o => o.FirstOrDefault(kv => kv.Value.SessionId == sessionId).Value);
            return session == default ? null : session;
        }

        public bool TryGetByConnectionId(Guid rmConnectionId, [NotNullWhen(true)] out ManagedSession? session)
        {
            session = _collection.Read(o =>
            {
                o.TryGetValue(rmConnectionId, out var session);
                return session;
            });
            return session != null;
        }

        public ManagedSession? GetByConnectionId(Guid rmConnectionId)
        {
            return _collection.Read(o =>
            {
                o.TryGetValue(rmConnectionId, out var session);
                return session;
            });
        }

        public void Delete(Guid rmConnectionId)
        {
            _collection.Write(o =>
            {
                if (o.TryGetValue(rmConnectionId, out var session))
                {
                    //TODO: Clean up session resources if needed.

                    Console.WriteLine($"Session ended with SessionId: {session.SessionId}");
                    o.Remove(rmConnectionId);
                }
            });
        }
    }
}
