using MODELS.BASE;
using MODELS.USER.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.MESSAGESTATUS.Dtos
{
    public class MODELConversation : MODELBase
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        /// <summary>
        /// 0: Converstation - User to User; 1: Group - User to Group
        /// </summary>
        public int TypeOfConversation { get; set; }

        /// <summary>
        /// Id của User hoặc của Group dựa theo TypeOfConversation
        /// </summary>
        public Guid TargetId { get; set; }

        public Guid? LastReadMessageId { get; set; }


        public MODELUser User { get; set; } = new MODELUser();
    }
}
