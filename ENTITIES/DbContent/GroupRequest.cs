﻿using System;
using System.Collections.Generic;

namespace ENTITIES.DbContent;

public partial class GroupRequest
{
    public Guid Id { get; set; }

    public Guid GroupId { get; set; }

    public Guid SenderId { get; set; }

    public Guid ReceiverId { get; set; }

    /// <summary>
    /// 0 - Đang chờ, 1 - Đồng ý, 2 - Từ chối
    /// </summary>
    public int State { get; set; }

    public DateTime NgayTao { get; set; }

    public string NguoiTao { get; set; } = null!;

    public DateTime NgaySua { get; set; }

    public string NguoiSua { get; set; } = null!;

    public DateTime? NgayXoa { get; set; }

    public string? NguoiXoa { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActived { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual User Receiver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
