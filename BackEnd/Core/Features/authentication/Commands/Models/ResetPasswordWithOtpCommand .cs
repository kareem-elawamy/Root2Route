using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.Authentication.Commands.Models
{
    public class ResetPasswordWithOtpCommand : IRequest<Response<string>>
    {
        public string Email { get; set; }

        public string Otp { get; set; }

        public string NewPassword { get; set; }
    }
}