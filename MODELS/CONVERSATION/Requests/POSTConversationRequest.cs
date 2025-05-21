using FluentValidation;
using MODELS.BASE;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.MESSAGESTATUS.Requests
{
    public class POSTConversationRequest : BaseRequest
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "UserId không được để trống")]
        public Guid UserId { get; set; }

        /// <summary>
        /// 0: Converstation - User to User; 1: Group - User to Group
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "TypeOfMessage không được để trống")]
        public int TypeOfMessage { get; set; }

        /// <summary>
        /// Id của User hoặc của Group dựa theo TypeOfMessage
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "TargetId không được để trống")]
        public Guid TargetId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "IsRead không được để trống")]
        public bool IsRead { get; set; }

        public DateTime? ReadAt { get; set; }

        public int? UnreadCount { get; set; }
    }

    public class POSTMessageStatusRequestValidator : AbstractValidator<POSTConversationRequest>
    {
        public POSTMessageStatusRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId không được để trống");
            RuleFor(x => x.TypeOfMessage).NotEmpty().WithMessage("TypeOfMessage không được để trống");
            RuleFor(x => x.TargetId).NotEmpty().WithMessage("TargetId không được để trống");
            RuleFor(x => x.IsRead).NotEmpty().WithMessage("IsRead không được để trống");
        }
    }
}
