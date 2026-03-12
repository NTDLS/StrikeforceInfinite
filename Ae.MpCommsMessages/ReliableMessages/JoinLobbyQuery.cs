using NTDLS.ReliableMessaging;
using System;

namespace Ae.MpCommsMessages.ReliableMessages
{
    /// <summary>
    /// Represents a query to join a lobby using its unique identifier.
    /// </summary>
    /// <remarks>Use this type to request joining a specific lobby. The query must specify the lobby's unique
    /// identifier via the LobbyId property. This type is typically used in remote messaging scenarios to initiate a
    /// join operation for a lobby.</remarks>
    public class JoinLobbyQuery
        : IRmQuery<JoinLobbyQueryReply>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the lobby.
        /// </summary>
        public Guid LobbyId { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public JoinLobbyQuery()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public JoinLobbyQuery(Guid lobbyId)
        {
            LobbyId = lobbyId;
        }
    }

    /// <summary>
    /// Represents the result of a join lobby query, including any error information encountered during the operation.
    /// </summary>
    /// <remarks>This class is used to convey the outcome of a join lobby request in multiplayer scenarios. It
    /// implements both IRmQueryReply and IMultiPlayQueryReply interfaces, allowing it to be used in contexts where
    /// either reply type is expected.</remarks>
    public class JoinLobbyQueryReply
        : IRmQueryReply, IMultiPlayQueryReply
    {
        /// <summary>
        /// Gets or sets the error message associated with the current operation.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public JoinLobbyQueryReply()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public JoinLobbyQueryReply(Exception ex)
        {
            ErrorMessage = ex.GetBaseException().Message;
        }
    }
}
