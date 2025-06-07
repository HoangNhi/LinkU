using MODELS.GROUP.Dtos;

namespace MODELS.CONVERSATION.Dtos
{
    public class MODELConversationGetListPaging
    {
        public Guid TargetId { get; set; } = Guid.Empty;
        public string TargetName { get; set; } = string.Empty;
        public string TargetPicture { get; set; } = string.Empty;
        public int TypeOfConversation { get; set; } = 0; // 0: 1-1, 1: Group
        public bool IsRead => UnreadCount == 0;
        public int UnreadCount { get; set; } = 1;
        public string UserSendLastestMessage { get; set; } = string.Empty;
        public string LatestMessage { get; set; } = string.Empty;
        /// <summary>
        /// 0 - tin nhắn thông thường, 1 - Tin nhắn chào mừng( sử dụng khi tạo group), 2 - Tin nhắn là File, 3 - Tin nhắn vừa text và file, 4 - Tin nhắn là 1 cuộc gọi điện
        /// </summary>
        public int LatestMessageType { get; set; }
        public DateTime LatestMessageDate { get; set; } = DateTime.Now;

        public MODELGroupAvartar Avartar { get; set; } = new MODELGroupAvartar();
    }
}
