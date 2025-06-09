using MODELS.BASE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.MESSAGE.Requests
{
    public class GetMessageByIdAndTypeRequest : GetByIdRequest
    {
        public int MessageType { get; set; } = 0;
    }
}
