using MODELS.BASE;
using MODELS.MEDIAFILE.Dtos;
using MODELS.USER.Dtos;

namespace MODELS.REACTIONTYPE.Dtos
{
    public class ModelReactionType : MODELBase
    {
        public Guid Id { get; set; }
        public string TenGoi { get; set; } = null!;
        public int SapXep { get; set; }
        public string MediaFileJson { get; set; } = null!;
        public MODELMediaFile MediaFile { get; set; } = null!;
    }
}
