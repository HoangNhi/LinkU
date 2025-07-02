using FluentValidation;
using MODELS.BASE;
using System.ComponentModel.DataAnnotations;

namespace MODELS.MESSAGEREACTION.Requests
{
    public class POSTMessageReactionGetListPagingRequest : GetListPagingRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Tin nhắn không được để trống")]
        public Guid MessageId { get; set; }

        public Guid? ReactionTypeId { get; set; }
    }

    public class POSTMessageReactionGetListPagingRequestValidator : AbstractValidator<POSTMessageReactionGetListPagingRequest>
    {
        public POSTMessageReactionGetListPagingRequestValidator()
        {
            RuleFor(x => x.MessageId).NotEmpty().WithMessage("Tin nhắn không được để trống");
            RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(1).WithMessage("Số trang phải lớn hơn 0");
            RuleFor(x => x.RowPerPage).GreaterThanOrEqualTo(1).WithMessage("Số bản ghi trên trang phải lớn hơn 0");
        }
    }
}
