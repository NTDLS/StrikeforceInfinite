using NTDLS.Semaphore;

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
