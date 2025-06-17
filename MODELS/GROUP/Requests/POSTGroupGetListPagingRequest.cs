using FluentValidation;
using MODELS.BASE;
using System.ComponentModel.DataAnnotations;

namespace MODELS.GROUP.Requests
{
    public class POSTGroupGetListPagingRequest : GetListPagingRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Người dùng không được để trống")]
        public Guid UserId { get; set; }
    }

    public class POSTGroupGetListPagingRequestValidator : AbstractValidator<POSTGroupGetListPagingRequest>
    {
        public POSTGroupGetListPagingRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Người dùng không được để trống");
            RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(1).WithMessage("Số trang phải lớn hơn 0");
            RuleFor(x => x.RowPerPage).GreaterThanOrEqualTo(1).WithMessage("Số bản ghi trên trang phải lớn hơn 0");
        }
    }
}
