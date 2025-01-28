using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.OTP.Requests
{
    public class VerifyOTPRequest
    {
        /// <summary>
        /// Email hoặc số điện thoại
        /// </summary>
        public Guid UserId { get; set; }
        public string Code { get; set; }
    }
}
