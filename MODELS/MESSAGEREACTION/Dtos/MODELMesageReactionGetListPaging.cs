using MODELS.REACTIONTYPE.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.MESSAGEREACTION.Dtos
{
    public class MODELMesageReactionGetListPaging
    {
        public List<ModelReactionType> ReactionTypes { get; set; } = new List<ModelReactionType>();
        public List<MODELMessageReaction> MessageReactions { get; set; } = new List<MODELMessageReaction>();
    }
}
