using FluentValidation;
using MODELS.BASE;
using System.ComponentModel.DataAnnotations;

namespace MODELS.GROUPMEMBER.Requests
{
    public class POSTGroupMemberRequest : BaseRequest
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Group không được để trống")]
        public Guid GroupId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "User không được để trống")]
        public Guid UserId { get; set; }

        /// <summary>
        /// 1 - Member, 2 - Admin
        /// </summary>
        public int Role { get; set; } = 1;
    }

    public class POSTGroupMemberRequestValidator : AbstractValidator<POSTGroupMemberRequest>
    {
        public POSTGroupMemberRequestValidator()
        {
            RuleFor(x => x.GroupId).NotEmpty().WithMessage("Group không được để trống");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User không được để trống");
        }
    }
}
