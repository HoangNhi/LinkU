using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.USER.Requests
{
    public class PostLogoutRequest
    {
        public Guid UserId { get; set; }
        public Guid RefreshToken { get; set; }
    }
}
