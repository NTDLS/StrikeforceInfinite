using NTDLS.ReliableMessaging;
using System;

namespace Ae.MpCommsMessages.ReliableMessages
{
    /// <summary>
    /// Represents a query to create a new lobby with a specified name and maximum number of players.
    /// </summary>
    /// <remarks>Use this type to initiate a request for creating a lobby in a multiplayer environment. The
    /// properties must be set before sending the query. This class is typically used in conjunction with a
    /// request/response system implementing IRmQuery.</remarks>
    public class CreateLobbyQuery
        : IRmQuery<CreateLobbyQueryReply>
    {
        /// <summary>
        /// Gets or sets the name of the lobby.
        /// </summary>
        public string LobbyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the maximum number of players allowed in the game.
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public CreateLobbyQuery()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public CreateLobbyQuery(string lobbyName, int maxPlayers)
        {
            LobbyName = lobbyName;
            MaxPlayers = maxPlayers;
        }
    }

    /// <summary>
    /// Represents the reply to a create lobby query, containing the lobby identifier and any error information.
    /// </summary>
    /// <remarks>This class is used as a response for operations that attempt to create a multiplayer lobby.
    /// It provides the unique lobby identifier if the operation succeeds, or an error message if it fails. Implements
    /// both IRmQueryReply and IMultiPlayQueryReply interfaces for compatibility with different query handling
    /// systems.</remarks>
    public class CreateLobbyQueryReply
        : IRmQueryReply, IMultiPlayQueryReply
    {
        /// <summary>
        /// Gets or sets the unique identifier for the lobby.
        /// </summary>
        public Guid LobbyId { get; set; }
        /// <summary>
        /// Gets or sets the error message associated with the current operation or state.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public CreateLobbyQueryReply()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public CreateLobbyQueryReply(Exception ex)
        {
            ErrorMessage = ex.GetBaseException().Message;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public CreateLobbyQueryReply(Guid lobbyId)
        {
            LobbyId = lobbyId;
        }
    }
}
