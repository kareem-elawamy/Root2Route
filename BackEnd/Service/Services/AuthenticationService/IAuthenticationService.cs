namespace Service.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        Task<JwtAuthResult> GenerateToken(ApplicationUser user, bool isRememberMe = false);
    }
}
