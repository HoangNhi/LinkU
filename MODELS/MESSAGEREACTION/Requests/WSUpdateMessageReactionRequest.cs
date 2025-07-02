using MODELS.MESSAGEREACTION.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.MESSAGEREACTION.Requests
{
    public class WSUpdateMessageReactionRequest
    {
        public Guid SenderId { get; set; }
        public Guid TargetId { get; set; }
        public int ConversationType { get; set; } = 0;
        public MODELMessageReaction? Reaction { get; set; }
    }
}
