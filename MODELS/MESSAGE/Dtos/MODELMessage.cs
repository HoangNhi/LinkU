using MODELS.BASE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.MESSAGE.Dtos
{
    public class MODELMessage : MODELBase
    {
        public Guid Id { get; set; }

        public Guid SenderId { get; set; }

        public Guid ReceiverId { get; set; }

        /// <summary>
        /// Id của tin nhắn được trả lời
        /// </summary>
        public Guid? RefId { get; set; }

        public string Content { get; set; } = null!;

        public bool IsCall { get; set; }

        public bool IsFile { get; set; }

        public string? TenFile { get; set; }

        public string? TenMoRong { get; set; }

        public double? DoLon { get; set; }

        public string? Url { get; set; }
    }
}
