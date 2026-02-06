using NTDLS.DatagramMessaging;
using NTDLS.Semaphore;

namespace Si.MpLibrary
{
    public class Lobby
    {
        public DmClient _dmClient;

        public Guid LobbyId { get; private set; } = Guid.NewGuid();

        public Session OwnerSession { get; private set; }

        /// <summary>
        /// Client sessions in this lobby, including the owner session.
        /// </summary>
        public OptimisticCriticalResource<Dictionary<Guid, Session>> Sessions { get; private set; } = new();

        public SpriteActionBuffer ActionBuffer { get; private set; } = new();

        public Lobby(Session ownerSession, DmClient dmClient)
        {
            OwnerSession = ownerSession;
            _dmClient = dmClient;
            AddSession(ownerSession);
        }

        public void AddSession(Session session)
        {
            Sessions.Write(o => o.Add(session.SessionId, session));
        }

        public void FlushActionBuffer()
        {
            var sessions = Sessions.Read(o => o.Select(u => u.Value));

            ActionBuffer.FlushSpriteVectorsToClients(_dmClient, sessions);
        }
    }
}
