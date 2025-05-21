using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.CONVERSATION.Requests
{
    public class WSPrivateMessageUpdateConversation
    {
        public Guid UserId { get; set; }
        public Guid TargetId { get; set; }
    }
}
