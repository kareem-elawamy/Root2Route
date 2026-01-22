using Core.Base;
using MediatR;
using Service;

namespace Core.Features.authentication.Commands.Models
{
    public class SignInCommand : IRequest<Response<JwtAuthResult>>
    {

        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsRememberMe { get; set; }
    }
}
