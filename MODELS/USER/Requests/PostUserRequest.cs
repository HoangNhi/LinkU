using FluentValidation;
using MODELS.BASE;
using MODELS.COMMON;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.USER.Requests
{
    public class PostUserRequest : BaseRequest
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Tên đăng nhặp không được để trống")]
        public string Username { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Họ lót không được để trống")]
        public string HoLot { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Tên không được để trống")]
        public string Ten { get; set; } = null!;

        public string? Email { get; set; }

        public string? SoDienThoai { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = null!;

        public string PasswordSalt { get; set; } = null!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Ngày sinh không được để trống")]
        public DateTime DateOfBirth { get; set; }

        public Guid RoleId { get; set; }

        public string? Bio { get; set; }

        public string? ProfilePicture { get; set; }

        public string? CoverPicture { get; set; }

        public string? FolderUploadCoverPicture { get; set; } = Guid.NewGuid().ToString();
    }

    public class PostUserRequestValidator : AbstractValidator<PostUserRequest>
    {
        public PostUserRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Tên đăng nhập không được để trống");
            RuleFor(x => x.HoLot).NotEmpty().WithMessage("Họ lót không được để trống");
            RuleFor(x => x.Ten).NotEmpty().WithMessage("Tên không được để trống");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Mật khẩu không được để trống");
            RuleFor(x => x.DateOfBirth).NotEmpty().WithMessage("Ngày sinh không được để trống");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Email không hợp lệ");
            RuleFor(x => x.SoDienThoai).Must(CommonFunc.IsValidPhone).WithMessage("Số điện thoại không hợp lệ");
        }
    }
}
