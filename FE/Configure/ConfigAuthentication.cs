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
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(MODELS.COMMON.CommonConst.ExpireRefreshToken);
                options.LoginPath = "/Account/Login";
            })
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = builder.Configuration["Google:ClientId"];
                googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
            });

            // Prevent Cross-Site Request Forgery (CSRF)
            builder.Services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
        }
    }
}
