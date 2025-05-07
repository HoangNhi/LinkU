namespace ENTITIES.DbContent;

public partial class Message
{
    public Guid Id { get; set; }

    public Guid SenderId { get; set; }

    public Guid ReceiverId { get; set; }

    /// <summary>
    /// Id của tin nhắn được trả lời
    /// </summary>
    public Guid? RefId { get; set; }

    public string Content { get; set; } = null!;

    public bool IsCall { get; set; }

    public bool IsFile { get; set; }

    public string? TenFile { get; set; }

    public string? TenMoRong { get; set; }

    public double? DoLon { get; set; }

    public string? Url { get; set; }

    public DateTime NgayTao { get; set; }

    public string NguoiTao { get; set; } = null!;

    public DateTime NgaySua { get; set; }

    public string NguoiSua { get; set; } = null!;

    public DateTime? NgayXoa { get; set; }

    public string? NguoiXoa { get; set; }

    public bool IsActived { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();

    public virtual User Receiver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
