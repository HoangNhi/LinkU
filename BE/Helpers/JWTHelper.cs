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
        public static string GenerateJwtToken(MODELUser User, IConfiguration Config)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Name, User.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, User.Email),
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
    }
}
