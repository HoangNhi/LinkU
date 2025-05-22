using FluentValidation;
using MODELS.BASE;
using System.ComponentModel.DataAnnotations;

namespace MODELS.GROUP.Requests
{
    public class POSTGroupRequest : BaseRequest
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Tên nhóm không được để trống")]
        public string GroupName { get; set; } = null!;

        /// <summary>
        /// True: public - Cho phép tham gia bằng Link, False: ngược lại
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Loại nhóm không được để trống")]
        public bool GroupType { get; set; } = true;
    }

    public class POSTGroupRequestValidator : AbstractValidator<POSTGroupRequest>
    {
        public POSTGroupRequestValidator()
        {
            RuleFor(x => x.GroupName).NotEmpty().WithMessage("Tên nhóm không được để trống");
            RuleFor(x => x.GroupType).NotNull().WithMessage("Loại nhóm không được để trống");
        }
    }
}
