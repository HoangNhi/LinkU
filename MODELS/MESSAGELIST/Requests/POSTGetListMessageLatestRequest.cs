using FluentValidation;
using MODELS.BASE;
using System.ComponentModel.DataAnnotations;

namespace MODELS.MESSAGELIST.Requests
{
    public class POSTGetListMessageLatestRequest : GetListPagingRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "CurrentUserId không được để trống")]
        public Guid CurrentUserId { get; set; }
    }

    public class POSTGetListMessageLatestRequestValidator : AbstractValidator<POSTGetListMessageLatestRequest>
    {
        public POSTGetListMessageLatestRequestValidator()
        {
            RuleFor(x => x.CurrentUserId).NotEmpty().WithMessage("CurrentUserId không được để trống");
        }
    }
}
