using System;
using System.Collections.Generic;

namespace ENTITIES.DbContent;

public partial class Group
{
    public Guid Id { get; set; }

    public string GroupName { get; set; } = null!;

    /// <summary>
    /// True: public - Cho phép tham gia bằng Link, False: ngược lại
    /// </summary>
    public bool GroupType { get; set; }

    public DateTime NgayTao { get; set; }

    public string NguoiTao { get; set; } = null!;

    public DateTime NgaySua { get; set; }

    public string NguoiSua { get; set; } = null!;

    public DateTime? NgayXoa { get; set; }

    public string? NguoiXoa { get; set; }

    public bool IsActived { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual ICollection<GroupRequest> GroupRequests { get; set; } = new List<GroupRequest>();

    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
}
