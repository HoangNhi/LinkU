using FluentValidation;
using MODELS.BASE;
using System.ComponentModel.DataAnnotations;

namespace MODELS.MESSAGEREACTION.Requests
{
    public class POSTMessageReactionRequest : BaseRequest
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Tin nhắn không được để trống")]
        public Guid MessageId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Người dùng không được để trống")]
        public Guid UserId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Loại phản ứng không được để trống")]
        public Guid ReactionTypeId { get; set; }
    }

    public class POSTMessageReactionRequestValidator : AbstractValidator<POSTMessageReactionRequest>
    {
        public POSTMessageReactionRequestValidator()
        {
            RuleFor(x => x.MessageId).NotEmpty().WithMessage("Tin nhắn không được để trống");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Người dùng không được để trống");
            RuleFor(x => x.ReactionTypeId).NotEmpty().WithMessage("Loại phản ứng không được để trống");
        }
    }
}
