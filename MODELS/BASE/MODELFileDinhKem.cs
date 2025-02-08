using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.BASE
{
    public class MODELFileDinhKem
    {
        public Guid Id { get; set; }
        public string? TenFile { get; set; }

        public string? TenMoRong { get; set; }

        public double? DoLon { get; set; }

        public string? Url { get; set; }
    }
}
