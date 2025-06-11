using FluentValidation;
using MODELS.BASE;
using System.ComponentModel.DataAnnotations;

namespace MODELS.GROUPREQUEST.Requests
{
    public class POSTGroupRequest : BaseRequest
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Nhóm không được để trống")]
        public Guid GroupId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Người gửi không được để trống")]
        public Guid SenderId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Người nhận không được để trống")]
        public Guid ReceiverId { get; set; }

        /// <summary>
        /// 0 - Đang chờ, 1 - Đồng ý, 2 - Từ chối
        /// </summary>
        public int State { get; set; } = 0;
    }

    public class POSTGroupRequestValidator : AbstractValidator<POSTGroupRequest>
    {
        public POSTGroupRequestValidator()
        {
            RuleFor(x => x.GroupId).NotEmpty().WithMessage("Nhóm không được để trống");
            RuleFor(x => x.SenderId).NotEmpty().WithMessage("Người gửi không được để trống");
            RuleFor(x => x.ReceiverId).NotEmpty().WithMessage("Người nhận không được để trống");
        }
    }
}
