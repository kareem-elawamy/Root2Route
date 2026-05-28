using API.Controllers.Shared;
using Core.Features.authentication.Commands.Models;
using Domain.MetaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    public class UserController : BaseApiController
    {
        [Authorize]
        [HttpDelete(Router.User.DeleteAccount)]
        public async Task<IActionResult> DeleteAccount()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            var command = new DeleteAccountCommand { UserId = userId };
            var response = await Mediator.Send(command);
            
            return NewResult(response);
        }
    }
}
