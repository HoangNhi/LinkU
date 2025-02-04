using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace MODELS.REFRESHTOKEN.Requests
{
    public class RefreshTokenRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Token không được để trống")]
        public string Token { get; set; }
    }

    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.Token).NotEmpty().WithMessage("Token không được để trống");
        }
    }
}
