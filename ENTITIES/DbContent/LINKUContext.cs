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

    public virtual DbSet<Friendship> Friendships { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<OTP> OTPs { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comment");

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

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.ToTable("Friendship");

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

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_like");

            entity.ToTable("Like");

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

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Message");

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
            entity.Property(e => e.RefId).HasComment("Id của tin nhắn được trả lời");
            entity.Property(e => e.TenMoRong)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Message_User1");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Message_User");
        });

        modelBuilder.Entity<OTP>(entity =>
        {
            entity.ToTable("OTP");

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

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Token).HasName("PK_RefreshToken_1");

            entity.ToTable("RefreshToken");

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

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CoverPicture).HasMaxLength(200);
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
            entity.Property(e => e.ProfilePicture).HasMaxLength(200);
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
