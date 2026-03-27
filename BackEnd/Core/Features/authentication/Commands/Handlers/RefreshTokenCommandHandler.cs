using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Authentication.Commands.Models;
using Domain.Models;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Service;
using Service.Services.AuthenticationService;

namespace Core.Features.Authentication.Commands.Handlers
{
    public class RefreshTokenCommandHandler : ResponseHandler, IRequestHandler<RefreshTokenCommand, Response<JwtAuthResult>>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthenticationService _authService;
        private readonly JwtSettings _jwtSettings;

        public RefreshTokenCommandHandler(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, 
            IAuthenticationService authService,
            IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _userManager = userManager;
            _authService = authService;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<Response<JwtAuthResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
            {
                return BadRequest<JwtAuthResult>("Invalid access token or token is severely malformed.");
            }

            var userIdString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return BadRequest<JwtAuthResult>("Invalid user in token.");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound<JwtAuthResult>("User not found.");

            // Security: Block banned/locked users
            if (await _userManager.IsLockedOutAsync(user))
                return BadRequest<JwtAuthResult>("Account is locked or banned.");

            // Security: Block unconfirmed email accounts
            if (!user.EmailConfirmed)
                return BadRequest<JwtAuthResult>("Email is not confirmed.");

            var currentRefreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == request.RefreshToken && r.UserId == userId, cancellationToken);

            if (currentRefreshToken == null || !currentRefreshToken.IsActive)
            {
                return BadRequest<JwtAuthResult>("Invalid or expired refresh token.");
            }

            // Revoke current token
            currentRefreshToken.RevokedOn = DateTime.UtcNow;
            _context.RefreshTokens.Update(currentRefreshToken);

            // Context Switching Logic
            // If the user requested a specific organization, use it. Otherwise, fallback to the old token's organization.
            Guid? orgId = request.OrganizationId;
            if (!orgId.HasValue)
            {
                var orgClaim = principal.FindFirst("organizationId")?.Value;
                if (!string.IsNullOrEmpty(orgClaim) && Guid.TryParse(orgClaim, out var parsedOrgId))
                    orgId = parsedOrgId;
            }

            var newTokens = await _authService.GenerateToken(user, orgId, isRememberMe: true);

            await _context.SaveChangesAsync(cancellationToken);

            return Success(newTokens);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateLifetime = false // Here we intentionally ignore the expiration
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token algorithm");

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
