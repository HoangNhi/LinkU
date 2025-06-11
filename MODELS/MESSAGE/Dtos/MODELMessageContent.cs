using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.MESSAGE.Dtos
{
    /// <summary>
    /// Sử dụng cho việc xử lý nội dung tin nhắn đối với các loại tin nhắn khác nhau.
    /// </summary>
    public class MODELMessageContent
    {
        public Guid UserId { get; set; }
        public List<Guid> TargetId { get; set; } = new List<Guid>();
    }
}
