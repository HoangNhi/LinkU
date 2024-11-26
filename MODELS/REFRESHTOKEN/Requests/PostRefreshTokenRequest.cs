using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.REFRESHTOKEN.Requests
{
    public class PostRefreshTokenRequest
    {
        public Guid UserId { get; set; }
        public Guid RefreshToken { get; set; }
    }
}
