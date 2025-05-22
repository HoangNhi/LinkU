using FluentValidation;
using Microsoft.AspNetCore.Http;
using MODELS.GROUPMEMBER.Requests;
using System.ComponentModel.DataAnnotations;

namespace MODELS.GROUP.Requests
{
    public class POSTCreateGroupRequest : POSTGroupRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Members không được để trống")]
        public List<POSTGroupMemberRequest> Members { get; set; } = new List<POSTGroupMemberRequest>();
        public IFormFile? Avatar { get; set; } = null!;
    }

    public class POSTCreateGroupRequestValidator : AbstractValidator<POSTCreateGroupRequest>
    {
        public POSTCreateGroupRequestValidator()
        {
            RuleFor(x => x.GroupName).NotEmpty().WithMessage("Tên nhóm không được để trống");
            RuleFor(x => x.GroupType).NotNull().WithMessage("Loại nhóm không được để trống");
            RuleFor(x => x.Members).NotEmpty().WithMessage("Members không được để trống");
        }
    }
}
