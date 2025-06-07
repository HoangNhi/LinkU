namespace BE.Helpers
{
    public static class CommonHelper
    {
        public static string GetClaim(this IHttpContextAccessor contextAccessor, string value)
        {
            return contextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(c => c.Type == value)?.Value ?? string.Empty;
        }
    }
}
