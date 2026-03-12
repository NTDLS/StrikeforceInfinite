using NTDLS.DatagramMessaging;
using NTDLS.Semaphore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ae.MpClientToServerComms
{
    /// <summary>
    /// An instance of a server lobby.
    /// </summary>
    public class ManagedLobby
    {
        private readonly DmMessenger _dmMessenger;

        /// <summary>
        /// Gets the unique identifier for the lobby instance.
        /// </summary>
        public Guid LobbyId { get; private set; } = Guid.NewGuid();
        /// <summary>
        /// Gets or sets the name associated with the object.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the maximum number of players allowed in the game.
        /// </summary>
        public int MaxPlayers { get; set; }
        /// <summary>
        /// Gets the session that owns this instance.
        /// </summary>
        public ManagedSession OwnerSession { get; private set; }

        /// <summary>
        /// Client sessions in this lobby, including the owner session.
        /// </summary>
        public OptimisticCriticalResource<Dictionary<Guid, ManagedSession>> Sessions { get; private set; } = new();

        /// <summary>
        /// Gets the buffer that stores pending sprite actions for processing.
        /// </summary>
        public SpriteActionBuffer ActionBuffer { get; private set; } = new();

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public ManagedLobby(ManagedSession ownerSession, DmMessenger dmMessenger)
        {
            OwnerSession = ownerSession;
            _dmMessenger = dmMessenger;
        }

        /// <summary>
        /// Adds a managed session to the collection using its session identifier.
        /// </summary>
        /// <param name="session">The managed session to add. Must not be null. The session's SessionId is used as the key in the collection.</param>
        public void AddSession(ManagedSession session)
        {
            Sessions.Write(o => o.Add(session.SessionId, session));
        }

        /// <summary>
        /// Flushes the action buffer and sends sprite vector updates to all connected client sessions.
        /// </summary>
        /// <remarks>Call this method to ensure that all pending sprite actions are transmitted to
        /// clients. This is typically used to synchronize client state with the server after a batch of actions has
        /// been accumulated.</remarks>
        public void FlushActionBuffer()
        {
            var sessions = Sessions.Read(o => o.Select(u => u.Value.DatagramEndPoint));

            ActionBuffer.FlushSpriteVectorsToClients(_dmMessenger, sessions);
        }
    }
}
