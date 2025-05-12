using FluentValidation;
using MODELS.BASE;
using System.ComponentModel.DataAnnotations;

namespace MODELS.CONVERSATION.Requests
{
    public class POSTSearchInConversationRequest : GetListPagingRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Vui lòng nhập từ khóa tìm kiếm")]
        public new string? TextSearch { get; set; } = string.Empty;
    }

    public class POSTSearchInConversationRequestValidator : AbstractValidator<POSTSearchInConversationRequest>
    {
        public POSTSearchInConversationRequestValidator()
        {
            RuleFor(x => x.TextSearch)
                .NotEmpty().WithMessage("Vui lòng nhập từ khóa tìm kiếm")
                .MaximumLength(100).WithMessage("Từ khóa tìm kiếm không được quá 100 ký tự");

            RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(1).WithMessage("Số trang phải lớn hơn 0");
            RuleFor(x => x.RowPerPage).GreaterThanOrEqualTo(1).WithMessage("Số bản ghi trên trang phải lớn hơn 0");

        }
    }

}
