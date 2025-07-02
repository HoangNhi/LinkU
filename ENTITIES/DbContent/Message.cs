using System;
using System.Collections.Generic;

namespace ENTITIES.DbContent;

public partial class Message
{
    public Guid Id { get; set; }

    public Guid SenderId { get; set; }

    public Guid TargetId { get; set; }

    /// <summary>
    /// Id của tin nhắn được trả lời
    /// </summary>
    public Guid? RefId { get; set; }

    public string? Content { get; set; }

    /// <summary>
    /// 0 - tin nhắn thông thường, 1 - Tin nhắn chào mừng( sử dụng khi tạo group), 2 - Tin nhắn thông báo các thay đổi của nhóm(đổi tên nhóm, thêm thành viên, chuyển nhóm trưởng), 3 - Tin nhắn là File, 4 - Tin nhắn vừa text và file, 5 - Tin nhắn là 1 cuộc gọi điện
    /// </summary>
    public int MessageType { get; set; }

    public DateTime NgayTao { get; set; }

    public string NguoiTao { get; set; } = null!;

    public DateTime NgaySua { get; set; }

    public string NguoiSua { get; set; } = null!;

    public DateTime? NgayXoa { get; set; }

    public string? NguoiXoa { get; set; }

    public bool IsActived { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();

    public virtual ICollection<MessageReaction> MessageReactions { get; set; } = new List<MessageReaction>();

    public virtual User Sender { get; set; } = null!;
}
