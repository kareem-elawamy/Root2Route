using Core.Base;
using Core.Features.authentication.Commands.Models;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Service;
using Service.Services.AuthenticationService;

namespace Core.Features.authentication.Commands.Handler
{
    public class SignInCommandHandler : ResponseHandler, IRequestHandler<SignInCommand, Response<JwtAuthResult>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthenticationService _authService;

        public SignInCommandHandler(UserManager<ApplicationUser> userManager,
                                    SignInManager<ApplicationUser> signInManager,
                                    IAuthenticationService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
        }

        public async Task<Response<JwtAuthResult>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
                return BadRequest<JwtAuthResult>("User not found");

            var signInResult = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!signInResult)
                return BadRequest<JwtAuthResult>("Password is not correct");
            if (!user.EmailConfirmed)
                return BadRequest<JwtAuthResult>("Email is not confirmed");

            var accessToken = await _authService.GenerateToken(user, null, request.IsRememberMe);

            return Success(accessToken);
        }
    }
}
