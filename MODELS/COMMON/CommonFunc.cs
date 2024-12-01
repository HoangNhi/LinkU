using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.RegularExpressions;

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

        public static string GetModelStateAPI(ModelStateDictionary modelState)
        {
            var errorList = (from item in modelState.Values
                             from error in item.Errors
                             select error.ErrorMessage).ToList();

            return errorList[0];
        }
        public static string GetModelState(ModelStateDictionary dic)
        {
            string error = "";
            var arrError = dic.Select(f => f.Value.Errors).Where(p => p.Count > 0).ToList();

            foreach (var item in arrError)
            {
                error += item[0].ErrorMessage + "<br />";
            }

            return error;
        }
    }
}
