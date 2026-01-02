using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Base;
using Domain.Enums;
using MediatR;

namespace Core.Features.authentication.Commands.Models
{
    public class AddUserCommand:IRequest<Response<string>>
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public UserType UserType { get; set; } = UserType.User;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? PhoneNumber { get; set; }
        
    }
}