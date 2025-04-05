using FluentValidation;
using MODELS.BASE;
using System.ComponentModel.DataAnnotations;

namespace MODELS.FRIENDSHIP.Requests
{
    public class POSTFriendshipRequest : BaseRequest
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "UserId1 không được để trống")]
        public Guid UserId1 { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "UserId2 không được để trống")]
        public Guid UserId2 { get; set; }
    }

    public class POSTFriendshipRequestValidator : AbstractValidator<POSTFriendshipRequest>
    {
        public POSTFriendshipRequestValidator()
        {
            RuleFor(x => x.UserId1).NotEmpty().WithMessage("UserId1 không được để trống");
            RuleFor(x => x.UserId2).NotEmpty().WithMessage("UserId2 không được để trống");
        }
    }
}
