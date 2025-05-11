using System;
using System.Collections.Generic;

namespace ENTITIES.DbContent;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string HoLot { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public string? Email { get; set; }

    public string? SoDienThoai { get; set; }

    public string Password { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public Guid RoleId { get; set; }

    public DateTime DateOfBirth { get; set; }

    public int Gender { get; set; }

    public string? Bio { get; set; }

    public bool IsGoogle { get; set; }

    public DateTime NgayTao { get; set; }

    public string NguoiTao { get; set; } = null!;

    public DateTime NgaySua { get; set; }

    public string NguoiSua { get; set; } = null!;

    public DateTime? NgayXoa { get; set; }

    public string? NguoiXoa { get; set; }

    public bool IsActived { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<FriendRequest> FriendRequests { get; set; } = new List<FriendRequest>();

    public virtual ICollection<Friendship> FriendshipUserId1Navigations { get; set; } = new List<Friendship>();

    public virtual ICollection<Friendship> FriendshipUserId2Navigations { get; set; } = new List<Friendship>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();

    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();

    public virtual ICollection<OTP> OTPs { get; set; } = new List<OTP>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual Role Role { get; set; } = null!;
}
