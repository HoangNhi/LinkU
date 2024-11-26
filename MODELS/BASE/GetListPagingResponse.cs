using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.BASE
{
    public class GetListPagingResponse
    {
        public int PageIndex { get; set; }
        public int TotalRow { get; set; }
        public object Data { get; set; }
    }
}
