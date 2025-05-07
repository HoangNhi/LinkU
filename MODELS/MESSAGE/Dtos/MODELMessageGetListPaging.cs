using MODELS.USER.Dtos;

namespace MODELS.MESSAGE.Dtos
{
    public class MODELMessageGetListPaging
    {
        public List<MODELMessage> Messages { get; set; } = new List<MODELMessage>();
        public MODELUser CurrentUser { get; set; } = new MODELUser();
        public MODELUser FriendUser { get; set; } = new MODELUser();
    }
}
