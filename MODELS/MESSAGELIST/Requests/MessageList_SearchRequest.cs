using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace MODELS.MESSAGELIST.Requests
{
    public class MessageList_SearchRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Vui lòng nhập từ khóa tìm kiếm")]
        public string TextSearch { get; set; }
    }

    public class MessageList_SearchRequestValidator : AbstractValidator<MessageList_SearchRequest>
    {
        public MessageList_SearchRequestValidator()
        {
            RuleFor(x => x.TextSearch).NotEmpty().WithMessage("Vui lòng nhập từ khóa tìm kiếm");
        }
    }
}
