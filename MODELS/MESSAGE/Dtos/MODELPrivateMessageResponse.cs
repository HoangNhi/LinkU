using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.MESSAGE.Dtos
{
    public class MODELMessageResponse
    {
        public Guid SenderId { get; set; }
        public Guid TargetId { get; set; }
        public string Message { get; set; }
        public string DateTime { get; set; }
    }
}
