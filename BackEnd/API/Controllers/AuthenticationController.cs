using API.Controllers.Shared;
using Core.Features.authentication.Commands.Models;
using Core.Features.Authentication.Commands.Models;
using Domain.MetaData;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class AuthenticationController : BaseApiController
    {
        [HttpPost(Router.Authentication.Regsiter)]
        public async Task<IActionResult> Regsiter([FromForm] AddUserCommand command)
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
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
    }
}