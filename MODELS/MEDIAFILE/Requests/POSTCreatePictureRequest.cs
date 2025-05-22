using Microsoft.AspNetCore.Http;
using MODELS.COMMON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.MEDIAFILE.Requests
{
    public class POSTCreatePictureRequest
    {
        public IFormFile File { get; set; }
        public Guid OwnerId { get; set; }
        public MediaFileType FileType { get; set; }
        public bool IsSaveChange { get; set; } = true;
    }
}
