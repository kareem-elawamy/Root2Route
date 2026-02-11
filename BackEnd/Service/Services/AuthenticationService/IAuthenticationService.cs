namespace Service.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        Task<JwtAuthResult> GenerateToken( ApplicationUser user,Guid? organizationId = null, bool isRememberMe = false);
    }
}
