using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AuthenticationService(
            IOptions<JwtSettings> jwtSettings,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
            _context = context;
        }

        public async Task<JwtAuthResult> GenerateToken(
            ApplicationUser user,
            Guid? organizationId = null,
            bool isRememberMe = false)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.UserName ?? "")

        };


            // ✅ 1. Identity Roles (Global)
            var identityRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in identityRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // ✅ 2. Organization Scope
            if (organizationId.HasValue)
            {
                claims.Add(new Claim("organizationId", organizationId.Value.ToString()));

                var membership = await _context.OrganizationMembers
                    .Include(m => m.OrganizationRole)
                        .ThenInclude(r => r.Permissions)
                    .FirstOrDefaultAsync(m =>
                        m.UserId == user.Id &&
                        m.OrganizationId == organizationId);

                if (membership?.OrganizationRole != null)
                {
                    claims.Add(new Claim("organizationRole",
                        membership.OrganizationRole.Name));

                    foreach (var permission in membership.OrganizationRole.Permissions)
                    {
                        claims.Add(new Claim("permission",
                            permission.PermissionsClaim));
                    }
                }
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Secret!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expireTime = isRememberMe
                ? DateTime.UtcNow.AddDays(15)
                : DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpireTime);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expireTime,
                signingCredentials: creds
            );

            return new JwtAuthResult
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireAt = token.ValidTo,
                FullName = user.FullName,
            };
        }
    }
}