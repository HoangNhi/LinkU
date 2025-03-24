using MODELS.BASE;
using System.ComponentModel.DataAnnotations;

namespace MODELS.FRIENDREQUEST.Requests
{
    public class POSTFriendRequest : BaseRequest
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Người gửi không được để trống")]
        public Guid SenderId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Người nhận không được để trống")]
        public Guid ReceiverId { get; set; }

        /// <summary>
        /// 0: Chưa xác nhận, 1: Đồng ý, 2: Từ chối 
        /// </summary>
        public int Status { get; set; } = 0;
    }
}
