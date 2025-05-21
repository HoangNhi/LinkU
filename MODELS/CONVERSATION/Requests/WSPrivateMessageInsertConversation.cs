using MODELS.USER.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.CONVERSATION.Requests
{
    public class WSPrivateMessageInsertConversation
    {
        public MODELUser Sender { get; set; }
        public MODELUser Receiver { get; set; }
        public Guid MessageId { get; set; }
    }
}
