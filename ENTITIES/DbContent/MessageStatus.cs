using System;
using System.Collections.Generic;

namespace ENTITIES.DbContent;

public partial class MessageStatus
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    /// <summary>
    /// 0: Converstation - User to User; 1: Group - User to Group
    /// </summary>
    public int TypeOfMessage { get; set; }

    /// <summary>
    /// Id của User hoặc của Group dựa theo TypeOfMessage
    /// </summary>
    public Guid TargetId { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public int? UnreadCount { get; set; }

    public DateTime NgayTao { get; set; }

    public string NguoiTao { get; set; } = null!;

    public DateTime NgaySua { get; set; }

    public string NguoiSua { get; set; } = null!;

    public DateTime? NgayXoa { get; set; }

    public string? NguoiXoa { get; set; }

    public bool IsActived { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User User { get; set; } = null!;
}
