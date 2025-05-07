namespace ENTITIES.DbContent;

public partial class Post
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string? Content { get; set; }

    public string? MediaType { get; set; }

    public string? MediaUrl { get; set; }

    public DateTime? NgayTao { get; set; }

    public string? NguoiTao { get; set; }

    public DateTime? NgaySua { get; set; }

    public string? NguoiSua { get; set; }

    public DateTime? NgayXoa { get; set; }

    public string? NguoiXoa { get; set; }

    public bool? IsActived { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual User? User { get; set; }
}
