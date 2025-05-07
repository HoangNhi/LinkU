using MODELS.BASE;

namespace MODELS.FRIENDREQUEST.Requests
{
    public class POSTFriendRequestGetListPagingRequest : GetListPagingRequest
    {
        // Receive/Send User Id
        public Guid UserId { get; set; }

        public bool IsSend { get; set; }
    }
}
