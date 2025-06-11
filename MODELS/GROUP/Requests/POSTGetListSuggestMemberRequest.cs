using FluentValidation;
using MODELS.BASE;
using System.ComponentModel.DataAnnotations;

namespace MODELS.GROUP.Requests
{
    public class POSTGetListSuggestMemberRequest : GetListPagingRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Người dùng không được để trống")]
        public Guid UserId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Nhóm không được để trống")]
        public Guid GroupId { get; set; }
    }

    public class POSTGetSuggestMemberRequestValidator : AbstractValidator<POSTGetListSuggestMemberRequest>
    {
        public POSTGetSuggestMemberRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Người dùng không được để trống");
            RuleFor(x => x.GroupId).NotEmpty().WithMessage("Nhóm không được để trống");
            RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(1).WithMessage("Số trang phải lớn hơn 0");
            RuleFor(x => x.RowPerPage).GreaterThanOrEqualTo(1).WithMessage("Số bản ghi trên trang phải lớn hơn 0");
        }
    }
}
