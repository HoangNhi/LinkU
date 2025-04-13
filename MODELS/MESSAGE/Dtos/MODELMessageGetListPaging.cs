using MODELS.BASE;
using MODELS.USER.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.MESSAGE.Dtos
{
    public class MODELMessageGetListPaging
    {
        public List<MODELMessage> Messages { get; set; } = new List<MODELMessage>();
        public MODELUser CurrentUser { get; set; } = new MODELUser();
        public MODELUser FriendUser { get; set; } = new MODELUser();
    }
}
