using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.MESSAGEREACTION.Requests
{
    public class GETMessageReactionShowPopupRequest
    {
        public List<ReactionTypeShowPopup> ReactionTypes { get; set; } = new List<ReactionTypeShowPopup>();
        public Guid MessageId { get; set; }
        public string ReactionTypesJSON { get; set; } = string.Empty;
    }

    public class ReactionTypeShowPopup
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
    }
}
