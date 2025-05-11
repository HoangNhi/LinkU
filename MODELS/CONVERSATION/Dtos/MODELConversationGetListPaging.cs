using MODELS.MESSAGE.Dtos;
using MODELS.MESSAGESTATUS.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.CONVERSATION.Dtos
{
    public class MODELConversationGetListPaging : MODELConversation
    {
        public bool IsRead { get; set; } = true;
        public int UnreadCount { get; set; } = 1;
        public MODELMessage LatestMessage { get; set; } = new MODELMessage();
        public bool IsMyMessage => this.LatestMessage.User.Id switch 
        {
            var id when id == Guid.Empty => false,
            _ => this.LatestMessage.User.Id == this.UserId
        };
    }
}
