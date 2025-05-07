using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace MODELS.MEDIAFILE.Requests
{
    public class POSTGetListMediaFilesRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "UserId không được để trống")]
        public Guid UserId { get; set; }
        /// <summary>
        /// Loại ảnh - Default là 0 - Profile Pciture
        /// 0 - Profile Picture, 1 - Cover Picture
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "FileType không được để trống")]
        public int FileType { get; set; } = 0;
    }

    public class POSTGetListMediaFilesRequestValidator : AbstractValidator<POSTGetListMediaFilesRequest>
    {
        public POSTGetListMediaFilesRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId không được để trống")
                .NotNull().WithMessage("UserId không được để trống");
            RuleFor(x => x.FileType)
                .NotNull().WithMessage("FileType không được để trống");
        }
    }
}
