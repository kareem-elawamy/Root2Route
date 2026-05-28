using Core.Base;
using Core.Features.authentication.Commands.Models;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Features.authentication.Commands.Handler
{
    public class ChangePasswordCommandHandler : ResponseHandler, IRequestHandler<ChangePasswordCommand, Response<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Response<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                return NotFound<string>("User not found.");
            }

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault()?.Description ?? "Failed to change password.";
                return BadRequest<string>(error);
            }

            return Success("Password changed successfully.");
        }
    }
}
