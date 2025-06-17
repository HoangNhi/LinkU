using MODELS.BASE;
using MODELS.GROUP.Dtos;
using MODELS.USER.Dtos;

namespace MODELS.GROUPREQUEST.Dtos
{
    public class MODELGroupRequestGetListPaging : MODELBase
    {
        public Guid Id { get; set; }

        public string GroupJson { get; set; } = null!;
        
        public string SendersJson { get; set; } = null!;

        public MODELGroup Group { get; set; } = new MODELGroup();

        public List<MODELUser> Senders { get; set; } = new List<MODELUser>();

        public Guid ReceiverId { get; set; }

        public int MemberCount { get; set; }

        #region Hàm hỗ trợ
        public int? Duration => (int)((DateTime.Now - NgaySua).TotalHours);
        public string DurationText => Duration switch
        {
            >= 24 => $"{Duration / 24} ngày",
            < 24 and >= 1 => $"{Duration} giờ",
            _ => "Vừa mới xảy ra"
        };
        #endregion
    }
}
