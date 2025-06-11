using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace MODELS.GROUPMEMBER.Requests
{
    public class POSTAddMemberToGroupRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Nhóm không được để trống")]
        public Guid GroupId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Danh sách thành viên không được để trống")]
        public List<Guid> UserIds { get; set; } = new List<Guid>();
    }

    public class POSTAddMemberToGroupRequestValidator : AbstractValidator<POSTAddMemberToGroupRequest>
    {
        public POSTAddMemberToGroupRequestValidator()
        {
            RuleFor(x => x.GroupId).NotEmpty().WithMessage("Nhóm không được để trống");
            RuleFor(x => x.UserIds).NotEmpty().WithMessage("Danh sách thành viên không được để trống");
        }
    }
}
