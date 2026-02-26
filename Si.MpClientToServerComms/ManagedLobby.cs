using NTDLS.DatagramMessaging;
using NTDLS.Semaphore;

namespace Si.MpClientToServerComms
{
    /// <summary>
    /// An instance of a server lobby.
    /// </summary>
    public class ManagedLobby
    {
        public DmMessenger _dmMessenger;

        public Guid LobbyId { get; private set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public int MaxPlayers { get; set; }
        public ManagedSession OwnerSession { get; private set; }

        /// <summary>
        /// Client sessions in this lobby, including the owner session.
        /// </summary>
        public OptimisticCriticalResource<Dictionary<Guid, ManagedSession>> Sessions { get; private set; } = new();

        public SpriteActionBuffer ActionBuffer { get; private set; } = new();

        public ManagedLobby(ManagedSession ownerSession, DmMessenger dmMessenger)
        {
            OwnerSession = ownerSession;
            _dmMessenger = dmMessenger;
        }

        public void AddSession(ManagedSession session)
        {
            Sessions.Write(o => o.Add(session.SessionId, session));
        }

        public void FlushActionBuffer()
        {
            var sessions = Sessions.Read(o => o.Select(u => u.Value.DatagramEndPoint));

            ActionBuffer.FlushSpriteVectorsToClients(_dmMessenger, sessions);
        }
    }
}
