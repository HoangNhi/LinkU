﻿using MODELS.BASE;

namespace MODELS.USER.Dtos
{
    public class MODELUser : MODELBase
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

        public string? ProfilePicture { get; set; }

        public string? CoverPicture { get; set; }

        public bool IsGoogle { get; set; }

        public string? RefreshToken { get; set; }

        public string? AccessToken { get; set; }

        #region Hàm hỗ trợ
        public string HoVaTen => $"{HoLot} {Ten}";

        public string GenderString => Gender == 1 ? "Nam" : "Nữ";

        public string DateOfBirthString => DateOfBirth.ToString("dd/MM/yyyy");
        #endregion
    }
}
