using MODELS.BASE;

namespace MODELS.GROUPREQUEST.Dtos
{
    public class MODELGroupRequest : MODELBase
    {
        public Guid Id { get; set; }

        public Guid GroupId { get; set; }

        public Guid SenderId { get; set; }

        public Guid ReceiverId { get; set; }

        /// <summary>
        /// 0 - Đang chờ, 1 - Đồng ý, 2 - Từ chối
        /// </summary>
        public int State { get; set; }
    }
}
