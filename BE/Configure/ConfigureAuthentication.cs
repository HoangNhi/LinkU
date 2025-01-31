using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BE.Configure
{
    public static class ConfigureAuthentication
    {
        public static void ExecuteConfigAuthentication(this WebApplicationBuilder builder)
        {
            // Cấu hình JWT
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                        // Mặc định ClockSkew là 5 phút
                        // Nên khi token hết hạn, ta vẫn có thể sử dụng trong 5 phút
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }
    }
}
