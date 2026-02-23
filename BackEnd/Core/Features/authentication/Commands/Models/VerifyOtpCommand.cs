using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service;

namespace Core.Features.Authentication.Commands.Models
{
    public class VerifyOtpCommand : IRequest<Response<JwtAuthResult>>
    {
        public string Email { get; set; } = null!;
        public string Otp { get; set; } = null!;
        
    }
}