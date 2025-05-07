using MODELS.BASE;
using MODELS.USER.Dtos;

namespace MODELS.FRIENDREQUEST.Requests
{
    public class POSTFriendRequest : BaseRequest
    {
        public Guid Id { get; set; }

        public Guid? SenderId { get; set; }

        public Guid? ReceiverId { get; set; }

        /// <summary>
        /// 0: Chưa xác nhận, 1: Đồng ý, 2: Từ chối 
        /// </summary>
        public int Status { get; set; } = 0;

        public string? Message { get; set; }

        public MODELUser? User { get; set; }
    }
}
