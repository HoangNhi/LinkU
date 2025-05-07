using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace MODELS.USER.Requests
{
    public class UsernameRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email hoặc số điện thoại không được để trống")]
        public string? Username { get; set; }
    }

    public class UsernameRequestValidator : AbstractValidator<UsernameRequest>
    {
        public UsernameRequestValidator()
        {
            RuleFor(r => r.Username).NotEmpty().WithMessage("Email hoặc số điện thoại không được để trống");
        }
    }
}
