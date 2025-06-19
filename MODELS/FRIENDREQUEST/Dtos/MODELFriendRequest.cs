using MODELS.BASE;
using MODELS.USER.Dtos;

namespace MODELS.FRIENDREQUEST.Dtos
{
    public class MODELFriendRequest : MODELBase
    {
        public Guid Id { get; set; }

        public Guid SenderId { get; set; }

        public Guid ReceiverId { get; set; }

        /// <summary>
        /// 0: Chưa xác nhận, 1: Đồng ý, 2: Từ chối 
        /// </summary>
        public int Status { get; set; }

        public string? Message { get; set; }

        public MODELUser User { get; set; }

        #region Hàm hỗ trợ
        public int? Duration => (int)((System.DateTime.Now - NgayTao).TotalHours);
        public string DurationText => Duration switch
        {
            >= 24 => $"{Duration / 24} ngày",
            < 24 and >= 1 => $"{Duration} giờ",
            _ => "Vừa mới xảy ra"
        };
        #endregion
    }
}
