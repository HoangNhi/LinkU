using MODELS.BASE;

namespace MODELS.MEDIAFILE.Dtos
{
    public class MODELMediaFile : MODELBase
    {
        public Guid Id { get; set; }

        public string Url { get; set; } = null!;

        public string FileName { get; set; } = null!;

        /// <summary>
        /// Enum: 0 - ProfilePicture, 1 -  CoverPicture, 2 - ChatImage, 3 - ChatFile, 4 - Avartar Group, 5 - ChatVideo
        /// </summary>
        public int FileType { get; set; }

        public Guid? OwnerId { get; set; }

        public Guid? MessageId { get; set; }

        /// <summary>
        /// 0 - Invalid, 1 - Hình vuông, 2 - Hình chữ nhật ngang, 3 - Hình chữ nhật dọc
        /// </summary>
        public int? Shape { get; set; }
        public int? FileLength { get; set; }
        public string FileLengthText { get {
                string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                double len = FileLength.HasValue ? (double)FileLength : 0;
                int order = 0;

                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len = len / 1024;
                }

                return $"{len:0.##} {sizes[order]}";
            }
        }
    }
}
