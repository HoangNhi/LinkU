using FluentValidation;
using MODELS.BASE;
using System.ComponentModel.DataAnnotations;

namespace MODELS.CONVERSATION.Requests
{
    public class POSTConversationGetListPagingRequest : GetListPagingRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "CurrentUserId không được để trống")]
        public Guid CurrentUserId { get; set; }
    }

    public class POSTConversationGetListPagingRequestValidator : AbstractValidator<POSTConversationGetListPagingRequest>
    {
        public POSTConversationGetListPagingRequestValidator()
        {
            RuleFor(x => x.CurrentUserId).NotEmpty().WithMessage("CurrentUserId không được để trống");
        }
    }
}
