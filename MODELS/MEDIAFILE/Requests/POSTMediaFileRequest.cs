using FluentValidation;
using MODELS.BASE;
using System.ComponentModel.DataAnnotations;

namespace MODELS.MEDIAFILE.Requests
{
    /// <summary>
    /// Sử dụng để xác nhận cập nhật hình ảnh của người dùng
    /// Dùng cho Profile và Cover Picture
    /// </summary>
    public class POSTMediaFileRequest : BaseRequest
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Url không được để trống")]
        public string Url { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "FileName không được để trống")]
        public string FileName { get; set; } = null!;

        /// <summary>
        /// Enum: 0 - ProfilePicture, 1 -  CoverPicture, 2 - ChatImage, 3 - ChatFile, 4 - Avartar Group
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "FileType không được để trống")]
        public int FileType { get; set; }

        public Guid? OwnerId { get; set; }

        public Guid? MessageId { get; set; }
    }

    public class POSTUserPictureRequestValidator : AbstractValidator<POSTMediaFileRequest>
    {
        public POSTUserPictureRequestValidator()
        {
            RuleFor(x => x.Url)
                .NotEmpty()
                .WithMessage("Url không được để trống");
            RuleFor(x => x.FileName)
                .NotEmpty()
                .WithMessage("FileName không được để trống");
            RuleFor(x => x.FileType)
                .NotNull()
                .WithMessage("FileType không được để trống");
        }
    }
}
