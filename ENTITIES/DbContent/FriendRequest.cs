using System;
using System.Collections.Generic;

namespace ENTITIES.DbContent;

public partial class FriendRequest
{
    public Guid Id { get; set; }

    public Guid SenderId { get; set; }

    public Guid ReceiverId { get; set; }

    /// <summary>
    /// 0: Chưa xác nhận, 1: Đồng ý, 2: Từ chối 
    /// </summary>
    public int Status { get; set; }

    public string? Message { get; set; }

    public DateTime NgayTao { get; set; }

    public string NguoiTao { get; set; } = null!;

    public DateTime NgaySua { get; set; }

    public string NguoiSua { get; set; } = null!;

    public DateTime? NgayXoa { get; set; }

    public string? NguoiXoa { get; set; }

    public bool IsActived { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User Sender { get; set; } = null!;
}
