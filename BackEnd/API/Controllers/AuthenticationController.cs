using API.Controllers.Shared;
using Core.Features.authentication.Commands.Models;
using Core.Features.Authentication.Commands.Models;
using Domain.MetaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class AuthenticationController : BaseApiController
    {
        [HttpPost(Router.Authentication.Regsiter)]
        public async Task<IActionResult> Regsiter([FromBody] AddUserCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpPost(Router.Authentication.Login)]
        public async Task<IActionResult> SignIn([FromBody] SignInCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpPost(Router.Authentication.verifyOtp)]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpPost(Router.Authentication.resendOtp)]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpPost(Router.Authentication.ForgetPassword)]
        public async Task<IActionResult> ForgetPassword([FromBody] SendResetPasswordOtpCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpPost(Router.Authentication.ResetPassword)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordWithOtpCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [Authorize]
        [HttpPost(Router.Authentication.ChangePassword)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            command.UserId = userId;
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
    }
}