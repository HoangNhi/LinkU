using MODELS.BASE;

namespace MODELS.GROUPMEMBER.Dtos
{
    public class MODELGroupMember : MODELBase
    {
        public Guid Id { get; set; }

        public Guid GroupId { get; set; }

        public Guid UserId { get; set; }

        /// <summary>
        /// 1 - Member, 2 - Admin
        /// </summary>
        public int Role { get; set; } = 1;
    }
}
