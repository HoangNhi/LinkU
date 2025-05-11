using System;
using System.Collections.Generic;

namespace ENTITIES.DbContent;

public partial class Conversation
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    /// <summary>
    /// 0: Converstation - User to User; 1: Group - User to Group
    /// </summary>
    public int TypeOfConversation { get; set; }

    /// <summary>
    /// Id của User hoặc của Group dựa theo TypeOfConversation
    /// </summary>
    public Guid TargetId { get; set; }

    public Guid? LastReadMessageId { get; set; }

    public DateTime NgayTao { get; set; }

    public string NguoiTao { get; set; } = null!;

    public DateTime NgaySua { get; set; }

    public string NguoiSua { get; set; } = null!;

    public DateTime? NgayXoa { get; set; }

    public string? NguoiXoa { get; set; }

    public bool IsActived { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Message? LastReadMessage { get; set; }

    public virtual User User { get; set; } = null!;
}
