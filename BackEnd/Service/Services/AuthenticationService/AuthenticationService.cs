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
                var membership = await _context.OrganizationMembers
                    .Where(m => m.UserId == user.Id && m.OrganizationId == organizationId.Value)
                        .Include(r => r.OrganizationRoles).ThenInclude(or => or.Permissions)
                    .FirstOrDefaultAsync(m =>
                        m.UserId == user.Id &&
                        m.OrganizationId == organizationId);

                if (membership != null)
                {
                    // Secure: Only add the organizationId claim if they are a confirmed member
                    claims.Add(new Claim("organizationId", organizationId.Value.ToString()));

                    if (membership.OrganizationRoles != null)
                    {
                        claims.Add(new Claim("organizationRole",
                            membership.OrganizationRoles.FirstOrDefault()?.Name ?? "Member"));

                        foreach (var permission in membership.OrganizationRoles.SelectMany(r => r.Permissions))
                        {
                            claims.Add(new Claim("permission", permission.PermissionsClaim));
                        }
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
                RefreshToken = (await CreateRefreshToken(user, organizationId)).Token
            };
        }
        private async Task<RefreshToken> CreateRefreshToken(ApplicationUser user, Guid? organizationId)
        {
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                CreatedOn = DateTime.UtcNow,
            };
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }
    }
}