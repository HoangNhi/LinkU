using MODELS.BASE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.FRIENDSHIP.Dtos
{
    public class MODELFriendship : MODELBase
    {
        public Guid Id { get; set; }

        public Guid UserId1 { get; set; }

        public Guid UserId2 { get; set; }
    }
}
