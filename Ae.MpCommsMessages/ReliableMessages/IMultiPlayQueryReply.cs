namespace Ae.MpCommsMessages.ReliableMessages
{
    /// <summary>
    /// Represents the reply to a multi-play query operation, including error information if the query fails.
    /// </summary>
    public interface IMultiPlayQueryReply
    {
        /// <summary>
        /// Gets or sets the error message associated with the current operation.
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
