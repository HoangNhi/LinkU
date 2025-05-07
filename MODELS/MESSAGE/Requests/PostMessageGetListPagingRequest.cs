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
        [Required(AllowEmptyStrings = false, ErrorMessage = "Id Người dùng hiện tại không được để trống")]
        public Guid CurrentId { get; set; }

        /// <summary>
        /// Friend id
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Id Người nhắn tin không được để trống")]
        public Guid FriendId { get; set; }
    }

    public class PostMessageGetListPagingRequestValidator : AbstractValidator<PostMessageGetListPagingRequest>
    {
        public PostMessageGetListPagingRequestValidator()
        {
            RuleFor(x => x.CurrentId).NotEmpty().WithMessage("Id Người dùng hiện tại không được để trống");
            RuleFor(x => x.FriendId).NotEmpty().WithMessage("Id Người nhắn tin không được để trống");
        }
    }
}
