using Ae.MpCommsMessages.DatagramMessages;
using Ae.MpCommsMessages.ReliableMessages;
using NTDLS.DatagramMessaging;
using NTDLS.ReliableMessaging;
using System;

namespace Ae.MpClientToServerComms
{
    /// <summary>
    /// Used my any multiplayer client to manage communication with the multiplayer server.
    /// This class abstracts away the details of the underlying messaging systems (Datagram and Reliable)
    /// and provides a simple interface for sending queries and handling incoming messages.
    /// </summary>
    public class MpCommsManager
        : IDisposable
    {
        private readonly DmMessenger? _dmMessenger;
        private readonly RmClient _rmClient;
        private readonly DmContext? _serverEndpointContext;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the MpCommsManager class and establishes connections
        /// for both datagram and reliable messaging clients to the specified server endpoint.
        /// </summary>
        public MpCommsManager(string hostOrIpAddress, int port)
        {
            var udpPort = DmMessenger.GetRandomUnusedUdpPort();

            _dmMessenger = new DmMessenger(udpPort);
            _dmMessenger.OnException += (DmContext? context, Exception ex) =>
            {
                Console.WriteLine($"[Client - DM Client] Exception: {ex.GetBaseException().Message}");
            };
            Console.WriteLine($"Datagram messaging client listening on port {_dmMessenger.ListenPort}.");

            _rmClient = new RmClient();
            _rmClient.OnException += (RmContext? context, Exception ex, IRmPayload? payload) =>
            {
                Console.WriteLine($"[Client - RM Client] Exception: {ex.GetBaseException().Message}");
            };

            Console.WriteLine("Starting reliable messaging client.");
            _rmClient.Connect(hostOrIpAddress, port);

            _serverEndpointContext = _dmMessenger.GetEndpointContext(hostOrIpAddress, port);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the object and optionally releases the managed resources.
        /// </summary>
        /// <remarks>This method is called by both the public Dispose method and the finalizer. When
        /// disposing is true, managed resources such as fields and properties should be released. When disposing is
        /// false, only unmanaged resources should be released. Derived classes should override this method to release
        /// additional resources as needed.</remarks>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _dmMessenger?.Dispose();
                    _rmClient.Disconnect();
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by the current instance of the class.
        /// </summary>
        /// <remarks>Call this method when you have finished using the object to free unmanaged and
        /// managed resources. After calling this method, the object should not be used further. This method is part of
        /// the IDisposable pattern and suppresses finalization for the object.</remarks>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Registers the specified message handler to process incoming Datagram messages.
        /// </summary>
        public void AddHandler(IDmDatagramHandler handler)
        {
            if (_dmMessenger == null)
                throw new Exception("Datagram messenger or is not initialized.");

            _dmMessenger.AddHandler(handler);
        }

        /// <summary>
        /// Registers the specified message handler to process incoming Reliable messages.
        /// </summary>
        public void AddHandler(IRmMessageHandler handler)
            => _rmClient.AddHandler(handler);

        /// <summary>
        /// Sets the current situation for the specified lobby.
        /// </summary>
        /// <param name="LobbyId">The unique identifier of the lobby for which the situation will be set.</param>
        /// <param name="situationName">The name of the situation to apply to the lobby. Cannot be null or empty.</param>
        /// <returns>A reply object containing the result of the situation update operation for the lobby.</returns>
        public SetSituationQueryReply SetSituation(Guid LobbyId, string situationName)
            => _rmClient.Query(new SetSituationQuery(LobbyId, situationName)).EnsureQuerySuccess();

        /// <summary>
        /// Starts a new game session for the specified lobby.
        /// </summary>
        /// <param name="LobbyId">The unique identifier of the lobby for which the game session should be started. Must reference an existing
        /// lobby.</param>
        /// <returns>A reply object containing the result of the start game operation. The reply indicates whether the game was
        /// successfully started.</returns>
        public StartGameQueryReply StartGame(Guid LobbyId)
            => _rmClient.Query(new StartGameQuery(LobbyId)).EnsureQuerySuccess();

        /// <summary>
        /// Starts a new server session and returns the result of the session initialization.
        /// </summary>
        /// <returns>A <see cref="StartServerSessionQueryReply"/> containing information about the newly started server session.
        /// The reply includes session details and status information.</returns>
        public StartServerSessionQueryReply StartServerSession()
            => _rmClient.Query(new StartServerSessionQuery()).EnsureQuerySuccess();

        /// <summary>
        /// Creates a new lobby with the specified name and maximum number of players.
        /// </summary>
        /// <param name="lobbyName">The name of the lobby to create. Cannot be null or empty.</param>
        /// <param name="maxPlayers">The maximum number of players allowed in the lobby. Must be greater than zero.</param>
        /// <returns>A reply object containing the result of the lobby creation request.</returns>
        public CreateLobbyQueryReply CreateLobby(string lobbyName, int maxPlayers)
            => _rmClient.Query(new CreateLobbyQuery(lobbyName, maxPlayers)).EnsureQuerySuccess();

        /// <summary>
        /// Retrieves a paged list of lobbies for the specified page number.
        /// </summary>
        /// <param name="pageNumber">The zero-based index of the page to retrieve. Must be greater than or equal to zero.</param>
        /// <returns>A reply object containing the lobbies for the requested page. The reply includes paging information and the
        /// list of lobbies for the specified page.</returns>
        public GetLobbiesPagedQueryReply GetLobbiesPaged(int pageNumber)
            => _rmClient.Query(new GetLobbiesPagedQuery(pageNumber)).EnsureQuerySuccess();

        /// <summary>
        /// Attempts to join the specified lobby and returns the result of the join operation.
        /// </summary>
        /// <param name="lobbyId">The unique identifier of the lobby to join. Must reference an existing lobby.</param>
        /// <returns>A JoinLobbyQueryReply containing the outcome of the join request. The reply includes information about the
        /// lobby and the user's join status.</returns>
        public JoinLobbyQueryReply JoinLobby(Guid lobbyId)
            => _rmClient.Query(new JoinLobbyQuery(lobbyId)).EnsureQuerySuccess();

        /// <summary>
        /// Attaches a datagram endpoint to the specified session.
        /// </summary>
        /// <param name="sessionId">The unique identifier of the session to which the datagram endpoint will be attached.</param>
        /// <exception cref="Exception">Thrown if the datagram messenger or server endpoint context is not initialized.</exception>
        public void AttachDatagramEndpointToSession(Guid sessionId)
        {
            if (_dmMessenger == null || _serverEndpointContext == null)
            {
                throw new Exception("Datagram messenger or server endpoint context is not initialized.");
            }
            _dmMessenger.Dispatch(new AttachDatagramEndpointToSessionMessage(sessionId), _serverEndpointContext);
        }
    }
}
