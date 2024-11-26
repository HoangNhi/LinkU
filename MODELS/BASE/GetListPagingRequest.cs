using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.BASE
{
    public class GetListPagingRequest
    {
        public string? TextSearch { get; set; } = string.Empty;
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int PageIndex { get; set; }
        public int RowPerPage { get; set; } = 10;
    }
}
