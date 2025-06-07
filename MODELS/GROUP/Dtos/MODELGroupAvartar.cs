using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.GROUP.Dtos
{
    public class MODELGroupAvartar
    {
        public List<string> UrlsAvartar { get; set; } = new List<string>();
        public int CountMember { get; set; } = 0;
    }
}
