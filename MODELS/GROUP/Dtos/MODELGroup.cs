using MODELS.BASE;
using MODELS.GROUPMEMBER.Dtos;

namespace MODELS.GROUP.Dtos
{
    public class MODELGroup : MODELBase
    {
        public Guid Id { get; set; }

        public string GroupName { get; set; } = null!;

        /// <summary>
        /// True: public - Cho phép tham gia bằng Link, False: ngược lại
        /// </summary>
        public bool GroupType { get; set; } = true;

        public List<MODELGroupMember> GroupMembers { get; set; } = new List<MODELGroupMember>();

        /// <summary>
        /// Url Avartar của nhóm, nếu không có thì sẽ sử dụng hình đại diện của các thành viên trong nhóm
        /// </summary>
        public string? AvartarUrl { get; set; }

        public MODELGroupAvartar Avartar { get; set; } = new MODELGroupAvartar();

        public int CountMember { get; set; } = 0;
    }
}
