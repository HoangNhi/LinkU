namespace ENTITIES.DbContent;

public partial class MediaFile
{
    public Guid Id { get; set; }

    public string Url { get; set; } = null!;

    public string FileName { get; set; } = null!;

    /// <summary>
    /// Enum: 1 - ProfilePicture, 2 -  CoverPicture, 3 - ChatImage, 4 - ChatFile
    /// </summary>
    public int FileType { get; set; }

    public Guid? OwnerId { get; set; }

    public Guid? MessageId { get; set; }

    public DateTime NgayTao { get; set; }

    public string NguoiTao { get; set; } = null!;

    public DateTime NgaySua { get; set; }

    public string NguoiSua { get; set; } = null!;

    public DateTime? NgayXoa { get; set; }

    public string? NguoiXoa { get; set; }

    public bool IsActived { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Message? Message { get; set; }

    public virtual User? Owner { get; set; }
}
