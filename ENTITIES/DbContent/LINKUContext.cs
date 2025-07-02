using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ENTITIES.DbContent;

public partial class LINKUContext : DbContext
{
    public LINKUContext(DbContextOptions<LINKUContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<FriendRequest> FriendRequests { get; set; }

    public virtual DbSet<Friendship> Friendships { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<GroupRequest> GroupRequests { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<MediaFile> MediaFiles { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MessageReaction> MessageReactions { get; set; }

    public virtual DbSet<OTP> OTPs { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<ReactionType> ReactionTypes { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comment");

            entity.HasIndex(e => e.PostId, "IX_Comment_PostId");

            entity.HasIndex(e => e.UserId, "IX_Comment_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayXoa).HasColumnType("datetime");
            entity.Property(e => e.NguoiSua)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiXoa)
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comment_Post");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comment_User");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_MessageStatus");

            entity.ToTable("Conversation");

            entity.HasIndex(e => e.LastReadMessageId, "IX_Conversation_LastReadMessageId");

            entity.HasIndex(e => e.UserId, "IX_Conversation_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayXoa).HasColumnType("datetime");
            entity.Property(e => e.NguoiSua)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiXoa)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.TargetId).HasComment("Id của User hoặc của Group dựa theo TypeOfConversation");
            entity.Property(e => e.TypeOfConversation).HasComment("0: Converstation - User to User; 1: Group - User to Group");

            entity.HasOne(d => d.LastReadMessage).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.LastReadMessageId)
                .HasConstraintName("FK_Conversation_Message");

            entity.HasOne(d => d.User).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conversation_User");
        });

        modelBuilder.Entity<FriendRequest>(entity =>
        {
            entity.ToTable("FriendRequest");

            entity.HasIndex(e => e.SenderId, "IX_FriendRequest_SenderId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Message).HasMaxLength(225);
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayXoa).HasColumnType("datetime");
            entity.Property(e => e.NguoiSua)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiXoa)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasComment("0: Chưa xác nhận, 1: Đồng ý, 2: Từ chối ");

            entity.HasOne(d => d.Sender).WithMany(p => p.FriendRequests)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FriendRequest_User");
        });

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.ToTable("Friendship");

            entity.HasIndex(e => e.UserId1, "IX_Friendship_UserId1");

            entity.HasIndex(e => e.UserId2, "IX_Friendship_UserId2");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayXoa).HasColumnType("datetime");
            entity.Property(e => e.NguoiSua)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiXoa)
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.HasOne(d => d.UserId1Navigation).WithMany(p => p.FriendshipUserId1Navigations)
                .HasForeignKey(d => d.UserId1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friendship_User");

            entity.HasOne(d => d.UserId2Navigation).WithMany(p => p.FriendshipUserId2Navigations)
                .HasForeignKey(d => d.UserId2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friendship_User1");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.ToTable("Group");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.GroupType).HasComment("True: public - Cho phép tham gia bằng Link, False: ngược lại");
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayXoa).HasColumnType("datetime");
            entity.Property(e => e.NguoiSua)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiXoa)
                .HasMaxLength(256)
                .IsUnicode(false);
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.ToTable("GroupMember");

            entity.HasIndex(e => e.GroupId, "IX_GroupMember_GroupId");

            entity.HasIndex(e => e.UserId, "IX_GroupMember_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayXoa).HasColumnType("datetime");
            entity.Property(e => e.NguoiSua)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiXoa)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasDefaultValue(1)
                .HasComment("1 - Member, 2 - Admin");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupMember_Group");

            entity.HasOne(d => d.User).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupMember_User");
        });

        modelBuilder.Entity<GroupRequest>(entity =>
        {
            entity.ToTable("GroupRequest");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayXoa).HasColumnType("datetime");
            entity.Property(e => e.NguoiSua)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiXoa)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.State).HasComment("0 - Đang chờ, 1 - Đồng ý, 2 - Từ chối");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupRequests)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupRequest_Group");

            entity.HasOne(d => d.Receiver).WithMany(p => p.GroupRequestReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupRequest_User1");

            entity.HasOne(d => d.Sender).WithMany(p => p.GroupRequestSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupRequest_User");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_like");

            entity.ToTable("Like");

            entity.HasIndex(e => e.CommentId, "IX_Like_CommentId");

            entity.HasIndex(e => e.PostId, "IX_Like_PostId");

            entity.HasIndex(e => e.UserId, "IX_Like_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayXoa).HasColumnType("datetime");
            entity.Property(e => e.NguoiSua)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiXoa)
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.HasOne(d => d.Comment).WithMany(p => p.Likes)
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("FK_Like_Comment");

            entity.HasOne(d => d.Post).WithMany(p => p.Likes)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Like_Post");

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Like_User");
        });

        modelBuilder.Entity<MediaFile>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_MediaFiles");

            entity.HasIndex(e => e.GroupId, "IX_MediaFiles_GroupId");

            entity.HasIndex(e => e.MessageId, "IX_MediaFiles_MessageId");

            entity.HasIndex(e => e.OwnerId, "IX_MediaFiles_OwnerId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FileType).HasComment("Enum: 1 - ProfilePicture, 2 -  CoverPicture, 3 - ChatImage, 4 - ChatFile, 5 - Avartar Group");
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayXoa).HasColumnType("datetime");
            entity.Property(e => e.NguoiSua)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NguoiXoa)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Shape).HasComment("0 - Invalid, 1 - Hình vuông, 2 - Hình chữ nhật ngang, 3 - Hình chữ nhật dọc");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Group).WithMany(p => p.MediaFiles)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK_MediaFiles_Group");

            entity.HasOne(d => d.Message).WithMany(p => p.MediaFiles)
                .HasForeignKey(d => d.MessageId)
                .HasConstraintName("FK_MediaFiles_Message");

            entity.HasOne(d => d.Owner).WithMany(p => p.MediaFiles)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_MediaFiles_User");

            entity.HasOne(d => d.ReactionType).WithMany(p => p.MediaFiles)
                .HasForeignKey(d => d.ReactionTypeId)
                .HasConstraintName("FK_MediaFiles_ReactionType");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Message");

            entity.HasIndex(e => e.TargetId, "IX_Message_ReceiverId");

            entity.HasIndex(e => e.SenderId, "IX_Message_SenderId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.MessageType).HasComment("0 - tin nhắn thông thường, 1 - Tin nhắn chào mừng( sử dụng khi tạo group), 2 - Tin nhắn thông báo các thay đổi của nhóm(đổi tên nhóm, thêm thành viên, chuyển nhóm trưởng), 3 - Tin nhắn là File, 4 - Tin nhắn vừa text và file, 5 - Tin nhắn là 1 cuộc gọi điện");
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayXoa).HasColumnType("datetime");
            entity.Property(e => e.NguoiSua)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiXoa)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.RefId).HasComment("Id của tin nhắn được trả lời");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Message_User");
        });

        modelBuilder.Entity<MessageReaction>(entity =>
        {
            entity.ToTable("MessageReaction");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayXoa).HasColumnType("datetime");
            entity.Property(e => e.NguoiSua)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiXoa)
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.HasOne(d => d.Message).WithMany(p => p.MessageReactions)
                .HasForeignKey(d => d.MessageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageReaction_Message");

            entity.HasOne(d => d.ReactionType).WithMany(p => p.MessageReactions)
                .HasForeignKey(d => d.ReactionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageReaction_ReactionType");

            entity.HasOne(d => d.User).WithMany(p => p.MessageReactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageReaction_User");
        });

        modelBuilder.Entity<OTP>(entity =>
        {
            entity.ToTable("OTP");

            entity.HasIndex(e => e.UserId, "IX_OTP_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Expires).HasColumnType("datetime");
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayXoa).HasColumnType("datetime");
            entity.Property(e => e.NguoiSua)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiXoa)
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.OTPs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OTP_User");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("Post");

            entity.HasIndex(e => e.UserId, "IX_Post_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.MediaType).HasMaxLength(50);
            entity.Property(e => e.MediaUrl).HasMaxLength(200);
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayXoa).HasColumnType("datetime");
            entity.Property(e => e.NguoiSua)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiXoa)
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Post_User");
        });

        modelBuilder.Entity<ReactionType>(entity =>
        {
            entity.ToTable("ReactionType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayXoa).HasColumnType("datetime");
            entity.Property(e => e.NguoiSua)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NguoiXoa)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TenGoi).HasMaxLength(255);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Token).HasName("PK_RefreshToken_1");

            entity.ToTable("RefreshToken");

            entity.HasIndex(e => e.UserId, "IX_RefreshToken_UserId");

            entity.Property(e => e.Token)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasComment("Refresh Token");
            entity.Property(e => e.Created)
                .HasComment("Lưu lại thời gian access token được cấp mới nhất để kiểm tra nếu token mới nhất chưa hết hạn mà lại được yêu cầu cấp 1 accesstoken mới thì sẽ trả về lỗi")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedByIp)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Expires).HasColumnType("datetime");
            entity.Property(e => e.ReplacedByToken)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Revoked).HasColumnType("datetime");
            entity.Property(e => e.RevokedByIp)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshToken_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayXoa).HasColumnType("datetime");
            entity.Property(e => e.NguoiSua)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiXoa)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.TenGoi).HasMaxLength(256);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.RoleId, "IX_User_RoleId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Gender).HasDefaultValue(1);
            entity.Property(e => e.HoLot).HasMaxLength(256);
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayXoa).HasColumnType("datetime");
            entity.Property(e => e.NguoiSua)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiTao)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.NguoiXoa)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Ten).HasMaxLength(256);
            entity.Property(e => e.Username)
                .HasMaxLength(256)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
