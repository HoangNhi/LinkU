using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGESTATUS.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.CONVERSATION.Dtos
{
    public class MODELConversationGetListPaging
    {
        public Guid TargetId { get; set; } = Guid.Empty;
        public string TargetName { get; set; } = string.Empty;
        public string TargetPicture { get; set; } = string.Empty;
        public int TypeOfConversation { get; set; } = 0; // 0: 1-1, 1: Group
        public bool IsRead { get; set; } = true;
        public int UnreadCount { get; set; } = 1;
        public string UserSendLastestMessage { get; set; } = string.Empty;
        public string LatestMessage { get; set; } = string.Empty;
        public DateTime LatestMessageDate { get; set; } = DateTime.Now;
    }
}
