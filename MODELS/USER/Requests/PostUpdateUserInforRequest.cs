using FluentValidation;
using MODELS.BASE;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.USER.Requests
{
    public class PostUpdateUserInforRequest : BaseRequest
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Bạn chưa nhập tên")]
        public string HoVaTen { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Ngày sinh không được để trống")]
        public DateTime DateOfBirth { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Giới tính không được để trống")]
        public int Gender { get; set; }

        public string? Email { get; set; }

        public string? SoDienThoai { get; set; }

        public string? ProfilePicture { get; set; }

        public string? CoverPicture { get; set; }

        public string GenderString => Gender == 1 ? "Nam" : "Nữ";

        public string DateOfBirthString => DateOfBirth.ToString("dd/MM/yyyy");
    }

    public class PostUpdateUserInforRequestValidator : AbstractValidator<PostUpdateUserInforRequest>
    {
        public PostUpdateUserInforRequestValidator()
        {
            RuleFor(r => r.HoVaTen).NotEmpty().WithMessage("Bạn chưa nhập tên");
            RuleFor(r => r.DateOfBirth).NotEmpty().WithMessage("Ngày sinh không được để trống");
            RuleFor(r => r.DateOfBirth)
                .NotEmpty().WithMessage("Ngày sinh không được để trống")
                .LessThan(DateTime.Now).WithMessage("Ngày sinh không hợp lệ");
        }
    }
}
