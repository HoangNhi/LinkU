﻿namespace MODELS.COMMON
{
    public static class CommonConst
    {
        public static int ExpireAccessToken = 1; // Thời hạn Access Token là 1 giờ
        public static int ExpireRefreshToken = 7; // Thời hạn Refresh Token là 7 ngày
        
        /// <summary>
        /// Thời hạn OTP là 5 phút
        /// </summary>
        public static int ExpireOTP = 5;
        
        /// <summary>
        /// Thời hạn đổi mật khẩu sau khi xác thực OTP là 10 phút
        /// </summary>
        public static int ExpireChangePassword = 10;
        
        /// <summary>
        /// Thời hạn lưu trữ thông tin người dùng trong Redis là 24 giờ
        /// </summary>
        public static int ExpireRedisUserProfile = 24 * 60;
        
        /// <summary>
        /// Thời hạn lưu trữ thông tin đã xử lý của MediaFile trong Redis là 24 giờ
        /// </summary>
        public static int ExpireRedisMediaFile = 24 * 60;
        
        /// <summary>
        /// Thời hạn lưu trữ thông tin đã xử lý của Message trong Redis là 24 giờ
        /// </summary>
        public static int ExpireRedisMessage = 24 * 60;

        /// <summary>
        /// Thời hạn lưu trữ thông tin đã xử lý của Group trong Redis là 24 giờ
        /// </summary>
        public static int ExpireRedisGroupMember = 24 * 60;

        /// <summary>
        /// Các định dạng file được phép upload
        /// Các trường hợp Upload: Profile Picture, Cover Picture, ChatImage, ChatFile
        /// </summary>
        //public static string[] AllowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf" };
        /// <summary>
        /// Sử dụng cho Profile Picture và Cover picture
        /// </summary>
        public static string[] AllowedPictureTypes = new[] { "image/jpeg", "image/png" };

        /// <summary>
        /// Số lượng thành viên tối đa 1 nhóm là 11 thành viên (1 Leader và 10 Member)
        /// </summary>
        public static int MaxGroupMember = 10;

        public static string DefaultUrlNoPicture = "https://linkv3.blob.core.windows.net/mediafiles/NoImage.jpg";
        public static string DefaultUrlNoCoverPicture = "https://linkv3.blob.core.windows.net/mediafiles/NoCoverPicture.jpg";
    }
}
