﻿using FluentValidation;
using MODELS.BASE;
using System.ComponentModel.DataAnnotations;

namespace MODELS.MESSAGE.Requests
{
    public class PostMessageRequest : BaseRequest
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Người gửi không được để trống")]
        public Guid SenderId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Người nhận không được để trống")]
        public Guid TargetId { get; set; }

        /// <summary>
        /// Id của tin nhắn được trả lời
        /// </summary>
        public Guid? RefId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Nội dung không được để trống")]
        public string? Content { get; set; }

        /// <summary>
        /// 0 - tin nhắn thông thường, 1 - Tin nhắn chào mừng( sử dụng khi tạo group), 2 - Tin nhắn thông báo các thay đổi của nhóm(đổi tên nhóm, thêm thành viên, chuyển nhóm trưởng), 3 - Tin nhắn là File, 4 - Tin nhắn vừa text và file, 5 - Tin nhắn là 1 cuộc gọi điện
        /// </summary>
        public int MessageType { get; set; } = 0;
    }

    public class PostMessageRequestValidator : AbstractValidator<PostMessageRequest>
    {
        public PostMessageRequestValidator()
        {
            RuleFor(x => x.SenderId).NotEmpty().WithMessage("Người gửi không được để trống");
            RuleFor(x => x.TargetId).NotEmpty().WithMessage("Người nhận không được để trống");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Nội dung không được để trống");
        }
    }
}
