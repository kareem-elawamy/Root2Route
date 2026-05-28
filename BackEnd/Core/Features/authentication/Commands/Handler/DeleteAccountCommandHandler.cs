using Core.Base;
using Core.Features.authentication.Commands.Models;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Features.authentication.Commands.Handler
{
    public class DeleteAccountCommandHandler : ResponseHandler, IRequestHandler<DeleteAccountCommand, Response<string>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteAccountCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Response<string>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            
            if (user == null)
            {
                return NotFound<string>("User not found.");
            }

            if (user.IsDeleted)
            {
                return BadRequest<string>("Account is already deleted.");
            }

            user.IsDeleted = true;
            
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest<string>("Failed to delete account.");
            }

            return Success("Account deleted successfully.");
        }
    }
}
