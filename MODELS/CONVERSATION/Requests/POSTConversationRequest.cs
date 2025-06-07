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
        public int TypeOfConversation { get; set; }

        /// <summary>
        /// Id của User hoặc của Group dựa theo TypeOfConversation
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "TargetId không được để trống")]
        public Guid TargetId { get; set; }

        public Guid? LastReadMessageId { get; set; }
    }

    public class POSTMessageStatusRequestValidator : AbstractValidator<POSTConversationRequest>
    {
        public POSTMessageStatusRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId không được để trống");
            RuleFor(x => x.TypeOfConversation).NotEmpty().WithMessage("Kiểu hội thoại không được để trống");
            RuleFor(x => x.TargetId).NotEmpty().WithMessage("TargetId không được để trống");
        }
    }
}
