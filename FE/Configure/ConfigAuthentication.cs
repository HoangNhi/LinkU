using Microsoft.AspNetCore.Authentication.Cookies;

namespace FE.Configure
{
    public static class ConfigAuthentication
    {
        public static void ConfigureAuthen(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(MODELS.COMMON.CommonConst.ExpireAccessToken);
                options.LoginPath = "/Account/Index";
            });

            // Prevent Cross-Site Request Forgery (CSRF)
            builder.Services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
        }
    }
}
