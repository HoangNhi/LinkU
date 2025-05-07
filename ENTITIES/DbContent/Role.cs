namespace ENTITIES.DbContent;

public partial class Role
{
    public Guid Id { get; set; }

    public string? TenGoi { get; set; }

    public string? GhiChu { get; set; }

    public string? NguoiTao { get; set; }

    public DateTime? NgayTao { get; set; }

    public string? NguoiSua { get; set; }

    public DateTime? NgaySua { get; set; }

    public string? NguoiXoa { get; set; }

    public DateTime? NgayXoa { get; set; }

    public bool? IsActived { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
