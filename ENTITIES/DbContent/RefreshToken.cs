namespace ENTITIES.DbContent;

public partial class RefreshToken
{
    /// <summary>
    /// Refresh Token
    /// </summary>
    public string Token { get; set; } = null!;

    public Guid UserId { get; set; }

    public DateTime Expires { get; set; }

    /// <summary>
    /// Lưu lại thời gian access token được cấp mới nhất để kiểm tra nếu token mới nhất chưa hết hạn mà lại được yêu cầu cấp 1 accesstoken mới thì sẽ trả về lỗi
    /// </summary>
    public DateTime Created { get; set; }

    public string CreatedByIp { get; set; } = null!;

    public DateTime? Revoked { get; set; }

    public string? RevokedByIp { get; set; }

    public string? ReplacedByToken { get; set; }

    public string? ReasonRevoked { get; set; }

    public virtual User User { get; set; } = null!;
}
