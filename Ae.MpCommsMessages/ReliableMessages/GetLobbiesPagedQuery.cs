using NTDLS.ReliableMessaging;
using Ae.MpCommsMessages.Models;

namespace Ae.MpCommsMessages.ReliableMessages
{
    public class GetLobbiesPagedQuery
        : IRmQuery<GetLobbiesPagedQueryReply>
    {
        public int PageNumber { get; set; } = 1;

        public GetLobbiesPagedQuery()
        {
        }

        public GetLobbiesPagedQuery(int pageNumber)
        {
            PageNumber = pageNumber;
        }
    }

    public class GetLobbiesPagedQueryReply
        : IRmQueryReply, IMultiPlayQueryReply
    {
        public int PageNumber { get; set; }
        public int TotalCountOfLobbies { get; set; }
        public string? ErrorMessage { get; set; }

        public Lobby[] Collection { get; set; } = [];

        public GetLobbiesPagedQueryReply()
        {
        }

        public GetLobbiesPagedQueryReply(Exception ex)
        {
            ErrorMessage = ex.GetBaseException().Message;
        }

        public GetLobbiesPagedQueryReply(Lobby[] collection, int pageNumber)
        {
            Collection = collection;
            PageNumber = pageNumber;
        }
    }
}
