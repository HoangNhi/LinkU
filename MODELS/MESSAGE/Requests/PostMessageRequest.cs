using FluentValidation;
using MODELS.BASE;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.MESSAGE.Requests
{
    public class PostMessageRequest : BaseRequest
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Người gửi không được để trống")]
        public Guid SenderId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Người nhận không được để trống")]
        public Guid ReceiverId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Nội dung không được để trống")]
        public string Content { get; set; } = null!;

        public bool IsCall { get; set; } = false;
    }

    public class PostMessageRequestValidator : AbstractValidator<PostMessageRequest>
    {
        public PostMessageRequestValidator()
        {
            RuleFor(x => x.SenderId).NotEmpty().WithMessage("Người gửi không được để trống");
            RuleFor(x => x.ReceiverId).NotEmpty().WithMessage("Người nhận không được để trống");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Nội dung không được để trống");
        }
    }
}
