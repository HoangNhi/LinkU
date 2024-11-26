using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MODELS.COMMON
{
    public static class CommonFunc
    {
        public static bool IsValidEmail(string email)
        {
            if (email == null) return false;

            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }


        public static bool IsValidPhone(string phoneNo)
        {
            var phoneNumber = phoneNo.Trim()
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("(", "")
                .Replace(")", "");
            return Regex.Match(phoneNumber, @"^\d{5,15}$").Success;
        }
    }
}
