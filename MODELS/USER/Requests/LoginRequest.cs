using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace MODELS.USER.Requests
{
    public class LoginRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email hoặc số điện thoại không được để trống")]
        public string? Username { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mật khẩu không được để trống")]
        public string? Password { get; set; }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(r => r.Username).NotEmpty().WithMessage("Email hoặc số điện thoại không được để trống");
            RuleFor(r => r.Password).NotEmpty().WithMessage("Mật khẩu không được để trống");
        }
    }
}
