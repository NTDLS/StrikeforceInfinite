using NTDLS.Semaphore;
using Si.MpComms;
using System.Diagnostics.CodeAnalysis;

namespace Si.MpServer
{
    internal class SessionManager(ServerInstance mpServerInstance)
    {
        //Dictionary of RmConnectionId to Session
        private readonly OptimisticCriticalResource<Dictionary<Guid, Session>> _collection = new();

        public Session Create(Guid rmConnectionId)
        {
            var session = new Session();

            _collection.Write(o =>
            {
                Console.WriteLine($"Session started with SessionId: {session.SessionId}");
                o.Add(rmConnectionId, session);
            });

            return session;
        }

        public bool TryGetBySessionId(Guid sessionId, [NotNullWhen(true)] out Session? session)
        {
            session = _collection.Read(o => o.FirstOrDefault(kv => kv.Value.SessionId == sessionId).Value);
            return session != default;
        }

        public Session? GetBySessionId(Guid sessionId)
        {
            var session = _collection.Read(o => o.FirstOrDefault(kv => kv.Value.SessionId == sessionId).Value);
            return session == default ? null : session;
        }

        public bool TryGetByConnectionId(Guid rmConnectionId, [NotNullWhen(true)] out Session? session)
        {
            session = _collection.Read(o =>
            {
                o.TryGetValue(rmConnectionId, out var session);
                return session;
            });
            return session != null;
        }

        public Session? GetByConnectionId(Guid rmConnectionId)
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
