using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.Authentication.Commands.Models
{
    public class ResendOtpCommand : IRequest<Response<string>>
    {
        public string Email { get; set; }
    }
}