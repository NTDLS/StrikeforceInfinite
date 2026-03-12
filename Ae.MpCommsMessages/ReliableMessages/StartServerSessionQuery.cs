using NTDLS.ReliableMessaging;
using System;

namespace Ae.MpCommsMessages.ReliableMessages
{
    /// <summary>
    /// Represents a query to initiate a server session and retrieve the result of the operation.
    /// </summary>
    /// <remarks>Use this type to request the start of a server session. The reply contains information about
    /// the session state and any relevant details. This query is typically used in scenarios where establishing a new
    /// server session is required before performing further operations.</remarks>
    public class StartServerSessionQuery
        : IRmQuery<StartServerSessionQueryReply>
    {
    }

    /// <summary>
    /// Represents the reply to a server session start query, containing the session identifier and any error
    /// information.
    /// </summary>
    /// <remarks>This class is used to convey the result of a request to start a server session. It provides
    /// the session ID if the operation succeeds, or an error message if it fails. Implements both IRmQueryReply and
    /// IMultiPlayQueryReply interfaces for compatibility with different query reply handling scenarios.</remarks>
    public class StartServerSessionQueryReply
        : IRmQueryReply, IMultiPlayQueryReply
    {
        /// <summary>
        /// Gets or sets the unique identifier for the current session.
        /// </summary>
        public Guid SessionId { get; set; }
        /// <summary>
        /// Gets or sets the error message associated with the current operation.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public StartServerSessionQueryReply()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public StartServerSessionQueryReply(Exception ex)
        {
            ErrorMessage = ex.GetBaseException().Message;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public StartServerSessionQueryReply(Guid sessionId)
        {
            SessionId = sessionId;
        }
    }
}
