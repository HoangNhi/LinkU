using MODELS.BASE;

namespace MODELS.FRIENDSHIP.Dtos
{
    public class MODELFriendship : MODELBase
    {
        public Guid Id { get; set; }

        public Guid UserId1 { get; set; }

        public Guid UserId2 { get; set; }
    }
}
