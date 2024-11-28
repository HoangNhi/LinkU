using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.USER.Requests
{
    public class RegisterRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Họ lót không được để trống")]
        public string HoLot { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Tên không được để trống")]
        public string Ten { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Ngày sinh không được để trống")]
        public DateTime DateOfBirth { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Số điện thoại hoặc email không được để trống")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Nhập lại mật khẩu không được để trống")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(r => r.HoLot).NotEmpty().WithMessage("Họ lót không được để trống");
            RuleFor(r => r.Ten).NotEmpty().WithMessage("Tên không được để trống");
            RuleFor(r => r.DateOfBirth).NotEmpty().WithMessage("Ngày sinh không được để trống");
            RuleFor(r => r.Username).NotEmpty().WithMessage("Số điện thoại hoặc email không được để trống");
            RuleFor(r => r.Password).NotEmpty().WithMessage("Mật khẩu không được để trống");
            RuleFor(r => r.ConfirmPassword).NotEmpty().WithMessage("Nhập lại mật khẩu không được để trống");
            RuleFor(r => r.ConfirmPassword).Equal(r => r.Password).WithMessage("Mật khẩu không khớp");
        }
    }
}
