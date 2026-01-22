using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        Task<JwtAuthResult> GenerateToken(ApplicationUser user);
    }
}
