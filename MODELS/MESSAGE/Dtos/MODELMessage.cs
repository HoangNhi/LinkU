using MODELS.BASE;
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
        /// 0 - tin nhắn thông thường, 1 - Tin nhắn chào mừng( sử dụng khi tạo group), 2 - Tin nhắn là File, 3 - Tin nhắn vừa text và file, 4 - Tin nhắn là 1 cuộc gọi điện
        /// </summary>
        public int MessageType { get; set; }

        public MODELUser Sender { get; set; } = null!;
    }
}
