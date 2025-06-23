using MODELS.BASE;
using MODELS.MEDIAFILE.Dtos;
using MODELS.USER.Dtos;

namespace MODELS.MESSAGE.Dtos
{
    public class MODELMessage : MODELBase
    {
        public Guid Id { get; set; }

        public Guid SenderId { get; set; }

        public Guid TargetId { get; set; }

        /// <summary>
        /// Id của tin nhắn được trả lời
        /// </summary>
        public Guid? RefId { get; set; }

        public string Content { get; set; } = null!;

        /// <summary>
        /// 0 - tin nhắn thông thường, 1 - Tin nhắn chào mừng( sử dụng khi tạo group), 2 - Tin nhắn thông báo các thay đổi của nhóm(đổi tên nhóm, thêm thành viên, chuyển nhóm trưởng), 3 - Tin nhắn là File, 4 - Tin nhắn vừa text và file, 5 - Tin nhắn là 1 cuộc gọi điện
        /// </summary>
        public int MessageType { get; set; }

        public MODELUser Sender { get; set; } = null!;
        public MODELMediaFile? MediaFile { get; set; } = null!;
    }
}
