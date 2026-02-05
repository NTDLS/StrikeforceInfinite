using NTDLS.Semaphore;

namespace Si.MpServer
{
    internal class LobbyManager(ServerInstance mpServerInstance)
    {
        //Dictionary of RmConnectionId to Lobby
        private readonly OptimisticCriticalResource<Dictionary<Guid, Lobby>> _collection = new();

        /*
        public ServerSession CreateSession(Guid rmConnectionId)
        {
            var session = new ServerSession();

            _sessions.Write(o =>
            {
                Console.WriteLine($"Session started with SessionId: {session.SessionId}");
                o.Add(rmConnectionId, session);
            });

            return session;
        }

        public void DeleteSession(Guid rmConnectionId)
        {
            _sessions.Write(o =>
            {
                if (o.TryGetValue(rmConnectionId, out var session))
                {
                    //TODO: Clean up session resources if needed.

                    Console.WriteLine($"Session ended with SessionId: {session.SessionId}");
                    o.Remove(rmConnectionId);
                }
            });
        }
    */
    }
}
