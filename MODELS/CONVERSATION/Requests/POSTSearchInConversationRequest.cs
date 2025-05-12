using FluentValidation;
using MODELS.BASE;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }

}
