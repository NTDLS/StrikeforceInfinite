using NTDLS.DatagramMessaging;
using NTDLS.ReliableMessaging;
using Si.MpCommsMessages.DatagramMessages;
using Si.MpCommsMessages.ReliableMessages;

namespace Si.MpClientToServerComms
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

        public SetSituationQueryReply SetSituation(Guid LobbyId, string situationName)
            => _rmClient.Query(new SetSituationQuery(LobbyId, situationName)).EnsureQuerySuccess();

        public StartGameQueryReply StartGame(Guid LobbyId)
            => _rmClient.Query(new StartGameQuery(LobbyId)).EnsureQuerySuccess();

        public StartServerSessionQueryReply StartServerSession()
            => _rmClient.Query(new StartServerSessionQuery()).EnsureQuerySuccess();

        public CreateLobbyQueryReply CreateLobby(string lobbyName, int maxPlayers)
            => _rmClient.Query(new CreateLobbyQuery(lobbyName, maxPlayers)).EnsureQuerySuccess();

        public GetLobbiesPagedQueryReply GetLobbiesPaged(int pageNumber)
            => _rmClient.Query(new GetLobbiesPagedQuery(pageNumber)).EnsureQuerySuccess();

        public JoinLobbyQueryReply JoinLobby(Guid lobbyId)
            => _rmClient.Query(new JoinLobbyQuery(lobbyId)).EnsureQuerySuccess();

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
