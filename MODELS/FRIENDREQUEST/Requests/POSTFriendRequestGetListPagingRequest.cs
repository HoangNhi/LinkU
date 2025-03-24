using MODELS.BASE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.FRIENDREQUEST.Requests
{
    public class POSTFriendRequestGetListPagingRequest : GetListPagingRequest
    {
        // Receive/Send User Id
        public Guid UserId { get; set; }

        public bool IsSend { get; set; }
    }
}
