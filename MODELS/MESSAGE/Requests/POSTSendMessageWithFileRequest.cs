using FluentValidation;
using Microsoft.AspNetCore.Http;
using MODELS.BASE;
using System.ComponentModel.DataAnnotations;

namespace MODELS.MESSAGE.Requests
{
    public class POSTSendMessageWithFileRequest : BaseRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Người gửi không được để trống")]
        public Guid SenderId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Người nhận không được để trống")]
        public Guid TargetId { get; set; }

        /// <summary>
        /// Id của tin nhắn được trả lời
        /// </summary>
        public Guid? RefId { get; set; }

        public string? Content { get; set; }

        public List<IFormFile> Files { get; set; } = new List<IFormFile>();

        public int ConversationType { get; set; } = 0;
    }

    public class POSTSendMessageWithFileRequestValidator : AbstractValidator<POSTSendMessageWithFileRequest>
    {
        public POSTSendMessageWithFileRequestValidator()
        {
            RuleFor(x => x.SenderId).NotEmpty().WithMessage("Người gửi không được để trống");
            RuleFor(x => x.TargetId).NotEmpty().WithMessage("Người nhận không được để trống");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Nội dung không được để trống");
            RuleFor(x => x.Files).NotEmpty().WithMessage("Vui lòng đính kèm ít nhất một tệp tin");
        }
    }
}
