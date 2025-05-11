using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.MESSAGELIST.Dtos
{
    public class MODELMessageList_GetListMessageLatest
    {
        public Guid UserId { get; set; } = Guid.Empty;
        public string HoLot { get; set; } = string.Empty;
        public string Ten { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        public string LatestMessage { get; set; } = string.Empty;
        public DateTime LatestMessageTime { get; set; }
        public bool IsMyMessage { get; set; } = false;

        // Chưa làm
        public bool IsRead { get; set; } = true;
        public int UnreadCount { get; set; } = 1;
    }
}
