using MODELS.BASE;
using MODELS.REACTIONTYPE.Dtos;
using MODELS.USER.Dtos;

namespace MODELS.MESSAGEREACTION.Dtos
{
    public class MODELMessageReaction : MODELBase
    {
        public Guid Id { get; set; }

        public Guid MessageId { get; set; }

        public Guid UserId { get; set; }

        public Guid ReactionTypeId { get; set; }

        public MODELUser User { get; set; } = null!;
        public string? UserJSON { get; set; }
        public string? URLReactionType { get; set; } = null!;

        // START Hiển thị ở danh sách tin nhắn
        public List<ModelReactionType> ReactionTypes { get; set; } = new List<ModelReactionType>();
        public List<MODELUser> ReactedUsers { get; set; } = new List<MODELUser>();
        public int ReactionCount { get; set; } = 0;
        public string ReactedUsername
        {
            get
            {
                var liststring = new List<string>();
                foreach (var item in ReactedUsers)
                {
                    liststring.Add(item.HoVaTen);
                }

                if (ReactionCount <= 3)
                {
                    return string.Join(", ", liststring);
                }
                else
                {
                    return $"{string.Join(", ", liststring)} và {ReactionCount - 3} người khác";
                }
            }
        }

        public Guid? MyReactionTypeId { get; set; }
        // END Hiển thị ở danh sách tin nhắn
    }
}
