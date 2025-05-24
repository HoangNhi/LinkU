using MODELS.USER.Dtos;

namespace MODELS.GROUP.Dtos
{
    public class MODELMemberCreateGroup : MODELUser
    {
        public char FirstCharacter => this.HoVaTen[0];
        public bool IsFriend { get; set; } = false;
    }
}
