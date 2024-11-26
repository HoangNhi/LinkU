using System;
using System.Collections.Generic;

namespace ENTITIES.DbContent;

public partial class RefreshToken
{
    /// <summary>
    /// Refresh Token
    /// </summary>
    public Guid RefreshToken1 { get; set; }

    public string AccessToken { get; set; } = null!;

    public Guid UserId { get; set; }

    public DateTime ExpiryDate { get; set; }

    /// <summary>
    /// Lưu lại thời gian access token được cấp mới nhất để kiểm tra nếu token mới nhất chưa hết hạn mà lại được yêu cầu cấp 1 accesstoken mới thì sẽ trả về lỗi
    /// </summary>
    public DateTime ExpiryDateAccessTokenRecent { get; set; }

    public bool IsActived { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User User { get; set; } = null!;
}
