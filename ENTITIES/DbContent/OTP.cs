﻿using System;
using System.Collections.Generic;

namespace ENTITIES.DbContent;

public partial class OTP
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Code { get; set; } = null!;

    public DateTime Expires { get; set; }

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
