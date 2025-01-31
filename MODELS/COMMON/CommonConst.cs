using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.COMMON
{
    public static class CommonConst
    {
        public static int ExpireAccessToken = 1; // Thời hạn Access Token là 1 giờ
        public static int ExpireRefreshToken = 7 * 24; // Thời hạn Refresh Token là 7 ngày
        /// <summary>
        /// Thời hạn OTP là 5 phút
        /// </summary>
        public static int ExpireOTP = 5;
        /// <summary>
        /// Thời hạn đổi mật khẩu sau khi xác thực OTP là 10 phút
        /// </summary>
        public static int ExpireChangePassword = 10;
    }
}
