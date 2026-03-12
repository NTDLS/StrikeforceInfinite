using NTDLS.ReliableMessaging;
using System;

namespace Ae.MpCommsMessages.ReliableMessages
{
    /// <summary>
    /// Represents a query to set the current situation for a specified lobby.
    /// </summary>
    /// <remarks>Use this query to update the situation associated with a lobby in the system. The query
    /// requires both the lobby identifier and the name of the situation to be set. This type is typically used in
    /// request/response messaging scenarios where the result is provided as a SetSituationQueryReply.</remarks>
    public class SetSituationQuery
        : IRmQuery<SetSituationQueryReply>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the lobby.
        /// </summary>
        public Guid LobbyId { get; set; }
        /// <summary>
        /// Gets or sets the name of the situation associated with this instance.
        /// </summary>
        public string SituationName { get; set; } = string.Empty;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public SetSituationQuery()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public SetSituationQuery(Guid lobbyId, string situationName)
        {
            LobbyId = lobbyId;
            SituationName = situationName;
        }
    }

    /// <summary>
    /// Represents the reply to a situation query operation, including error information if the query fails.
    /// </summary>
    /// <remarks>This class is used as a response type for situation query operations in multi-play and RM
    /// query scenarios. It provides error details when the operation does not succeed. Implementations of IRmQueryReply
    /// and IMultiPlayQueryReply allow integration with related query handling frameworks.</remarks>
    public class SetSituationQueryReply
        : IRmQueryReply, IMultiPlayQueryReply
    {
        /// <summary>
        /// Gets or sets the error message associated with the current operation.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public SetSituationQueryReply()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public SetSituationQueryReply(Exception ex)
        {
            ErrorMessage = ex.GetBaseException().Message;
        }
    }
}
