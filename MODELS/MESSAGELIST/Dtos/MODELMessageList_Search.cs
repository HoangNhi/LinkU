using MODELS.BASE;
using MODELS.MESSAGE.Dtos;
using MODELS.USER.Dtos;

namespace MODELS.MESSAGELIST.Dtos
{
    public class MODELMessageList_Search
    {
        // Người dùng
        public List<MODELUser> Users { get; set; } = new List<MODELUser>();

        // Tin nhắn
        public List<MODELMessage> Messages { get; set; } = new List<MODELMessage>();

        // Tệp đính kèm
        public List<MODELFileDinhKem> Files { get; set; } = new List<MODELFileDinhKem>();
    }
}
