using FluentValidation;
using MODELS.BASE;
using System.ComponentModel.DataAnnotations;

namespace MODELS.MESSAGE.Requests
{
    public class PostMessageGetListPagingRequest : GetListPagingRequest
    {
        /// <summary>
        /// Current user id
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Người dùng hiện tại không được để trống")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Target id
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mục tiêu nhận tin nhắn không được để trống")]
        public Guid TargetId { get; set; }

        /// <summary>
        /// Kiểu cuộc trò chuyện, 0: cá nhân, 1: nhóm
        /// </summary>
        public int ConversationType { get; set; } = 0;
    }

    public class PostMessageGetListPagingRequestValidator : AbstractValidator<PostMessageGetListPagingRequest>
    {
        public PostMessageGetListPagingRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Người dùng hiện tại không được để trống");
            RuleFor(x => x.TargetId).NotEmpty().WithMessage("Mục tiêu nhận tin nhắn không được để trống");
            RuleFor(x => x.PageIndex).NotNull().WithMessage("Kiểu hội thoại không được để trống");
        }
    }
}
