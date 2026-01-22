using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Service.Services.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly JwtSettings _jwtSettings;

        public AuthenticationService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<JwtAuthResult> GenerateToken(ApplicationUser user, bool isRememberMe = false)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim("UserType", user.UserType.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expireTime = isRememberMe
                     ? DateTime.UtcNow.AddDays(15)
                     : DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpireTime);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expireTime,
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new JwtAuthResult
            {
                AccessToken = accessToken,
                ExpireAt = tokenDescriptor.ValidTo,
                FullName = user.FullName,
                UserType = user.UserType.ToString()
            };
        }
    }
}