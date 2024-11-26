using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.REFRESHTOKEN.Dtos
{
    public class MODELRefreshToken
    {
        public string? AccessToken { get; set; }
        public Guid? RefreshToken { get; set; }
    }
}
