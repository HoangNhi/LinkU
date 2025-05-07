using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace MODELS.USER.Requests
{
    public class ChangePasswordRequest
    {
        public string Token { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.Password).NotEmpty().WithMessage("Mật khẩu không được để trống");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Xác nhận mật khẩu không được để trống");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Mật khẩu không trùng nhau");
        }
    }
}
