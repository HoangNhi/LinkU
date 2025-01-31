using ENTITIES.DbContent;
using Microsoft.IdentityModel.Tokens;
using MODELS.COMMON;
using MODELS.USER.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BE.Helpers
{
    public static class JWTHelper
    {
        /// <summary>
        /// Generate JWT Token For Login
        /// </summary>
        public static string GenerateJwtToken(MODELUser User, IConfiguration Config)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Name, User.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, User.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                Config["Jwt:Issuer"],
                Config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(CommonConst.ExpireAccessToken),
                signingCredentials: credentials);

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Generate JWT Token For Forgot Password
        /// </summary>
        /// <param name="Config"></param>
        /// <returns></returns>
        public static string GenerateForgetPasswordToken(MODELUser User, IConfiguration Config)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Name, User.Username),
                new Claim(JwtRegisteredClaimNames.Jti, User.Id.ToString()),
            };

            var token = new JwtSecurityToken(
                Config["Jwt:Issuer"],
                Config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(CommonConst.ExpireChangePassword),
                signingCredentials: credentials);

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Validate JWT Token For Forgot Password
        /// </summary>
        /// <param name="token"></param>
        /// <param name="Config"></param>
        /// <returns></returns>
        public static ClaimsPrincipal ValidateForgetPasswordToken(string token, IConfiguration Config)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Config["Jwt:Issuer"],
                ValidAudience = Config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["Jwt:Key"])),
                // Mặc định ClockSkew là 5 phút
                // Nên khi token hết hạn, ta vẫn có thể sử dụng trong 5 phút
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (Exception)
            {
                // Token validation failed
                return null;
            }
        }
    }
}
