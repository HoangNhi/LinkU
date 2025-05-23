﻿using FluentValidation;
using System.ComponentModel.DataAnnotations;

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

        [Required(AllowEmptyStrings = false, ErrorMessage = "Giới tính không được để trống")]
        public int? Gender { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email hoặc số điện thoại không được để trống")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Nhập lại mật khẩu không được để trống")]
        public string ConfirmPassword { get; set; }
        public bool IsGoogle { get; set; } = false;
    }

    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(r => r.HoLot).NotEmpty().WithMessage("Họ lót không được để trống");
            RuleFor(r => r.Ten).NotEmpty().WithMessage("Tên không được để trống");
            RuleFor(r => r.DateOfBirth).NotEmpty().WithMessage("Ngày sinh không được để trống");
            RuleFor(r => r.Gender).NotEmpty().WithMessage("Giới tính không được để trống");
            RuleFor(r => r.Username).NotEmpty().WithMessage("Email hoặc số điện thoại không được để trống");
            RuleFor(r => r.Password).NotEmpty().WithMessage("Mật khẩu không được để trống");
            RuleFor(r => r.ConfirmPassword).NotEmpty().WithMessage("Nhập lại mật khẩu không được để trống");
            RuleFor(r => r.ConfirmPassword).Equal(r => r.Password).WithMessage("Mật khẩu không khớp");
        }
    }
}
