using System;
using System.Collections.Generic;

namespace ENTITIES.DbContent;

public partial class ReactionType
{
    public Guid Id { get; set; }

    public string TenGoi { get; set; } = null!;

    public int SapXep { get; set; }

    public DateTime NgayTao { get; set; }

    public string NguoiTao { get; set; } = null!;

    public DateTime NgaySua { get; set; }

    public string NguoiSua { get; set; } = null!;

    public DateTime? NgayXoa { get; set; }

    public string? NguoiXoa { get; set; }

    public bool IsActived { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();

    public virtual ICollection<MessageReaction> MessageReactions { get; set; } = new List<MessageReaction>();
}
