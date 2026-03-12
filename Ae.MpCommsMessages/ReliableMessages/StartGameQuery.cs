using NTDLS.ReliableMessaging;
using System;

namespace Ae.MpCommsMessages.ReliableMessages
{
    /// <summary>
    /// Represents a query to initiate the start of a game within a specified lobby.
    /// </summary>
    /// <remarks>Use this query to request the transition from lobby state to active gameplay. The associated
    /// reply indicates whether the game was successfully started. This type is typically used in multiplayer scenarios
    /// where a lobby must be explicitly started before gameplay begins.</remarks>
    public class StartGameQuery
        : IRmQuery<StartGameQueryReply>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the lobby.
        /// </summary>
        public Guid LobbyId { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public StartGameQuery()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public StartGameQuery(Guid lobbyId)
        {
            LobbyId = lobbyId;
        }
    }

    /// <summary>
    /// Represents the reply to a start game query, including error information if the operation fails.
    /// </summary>
    /// <remarks>This class is used to convey the result of a start game operation in multiplayer scenarios.
    /// It implements both IRmQueryReply and IMultiPlayQueryReply interfaces to support different query reply contexts.
    /// If the operation fails, the ErrorMessage property contains details about the error.</remarks>
    public class StartGameQueryReply
        : IRmQueryReply, IMultiPlayQueryReply
    {
        /// <summary>
        /// Gets or sets the error message associated with the current operation.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public StartGameQueryReply()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public StartGameQueryReply(Exception ex)
        {
            ErrorMessage = ex.GetBaseException().Message;
        }
    }
}
