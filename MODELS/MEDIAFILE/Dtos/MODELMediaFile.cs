using MODELS.BASE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.MEDIAFILE.Dtos
{
    public class MODELMediaFile : MODELBase
    {
        public Guid Id { get; set; }

        public string Url { get; set; } = null!;

        public string FileName { get; set; } = null!;

        /// <summary>
        /// Enum: 1 - ProfilePicture, 2 -  CoverPicture, 3 - ChatImage, 4 - ChatFile
        /// </summary>
        public int FileType { get; set; }

        public Guid? OwnerId { get; set; }

        public Guid? MessageId { get; set; }
    }
}
