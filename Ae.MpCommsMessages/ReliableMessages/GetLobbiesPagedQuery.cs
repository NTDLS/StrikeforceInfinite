using Ae.MpCommsMessages.Models;
using NTDLS.ReliableMessaging;
using System;

namespace Ae.MpCommsMessages.ReliableMessages
{
    /// <summary>
    /// Represents a query for retrieving a paged list of lobbies.
    /// </summary>
    /// <remarks>Use this query to request a specific page of lobby data. The page number determines which
    /// subset of lobbies is returned. This type is typically used in scenarios where lobby data is too large to
    /// retrieve all at once and must be paged.</remarks>
    public class GetLobbiesPagedQuery
        : IRmQuery<GetLobbiesPagedQueryReply>
    {
        /// <summary>
        /// Gets or sets the current page number for paginated results.
        /// </summary>
        /// <remarks>The page number must be greater than zero. This property is typically used to specify
        /// which page of data to retrieve when working with paginated collections.</remarks>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public GetLobbiesPagedQuery()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public GetLobbiesPagedQuery(int pageNumber)
        {
            PageNumber = pageNumber;
        }
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public class GetLobbiesPagedQueryReply
        : IRmQueryReply, IMultiPlayQueryReply
    {
        /// <summary>
        /// Gets or sets the current page number in a paginated collection.
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// Gets or sets the total number of lobbies currently available.
        /// </summary>
        public int TotalCountOfLobbies { get; set; }
        /// <summary>
        /// Gets or sets the error message associated with the current operation.
        /// </summary>
        public string? ErrorMessage { get; set; }
        /// <summary>
        /// Gets or sets the collection of lobbies available in the current context.
        /// </summary>
        public Lobby[] Collection { get; set; } = [];

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public GetLobbiesPagedQueryReply()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public GetLobbiesPagedQueryReply(Exception ex)
        {
            ErrorMessage = ex.GetBaseException().Message;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public GetLobbiesPagedQueryReply(Lobby[] collection, int pageNumber)
        {
            Collection = collection;
            PageNumber = pageNumber;
        }
    }
}
