using Microsoft.AspNetCore.Mvc.Rendering;

namespace FE.Helpers
{
    public static class ClaimHelper
    {
        public static string GetClaimValue(this ViewContext viewContext, string claimType)
        {
            var claim = viewContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim != null ? claim.Value : string.Empty;
        }
    }
}
