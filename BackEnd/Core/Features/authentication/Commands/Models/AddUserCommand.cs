using Core.Base;
using Domain.Enums;
using MediatR;
using Service;

namespace Core.Features.authentication.Commands.Models
{
    public class AddUserCommand : IRequest<Response<JwtAuthResult>>
    {
        // بيانات الحساب الأساسية
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public UserType UserType { get; set; } 
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? PhoneNumber { get; set; }

        
        public string? OrganizationName { get; set; }
        public OrganizationType? OrganizationType { get; set; }
        public string? OrganizationAddress { get; set; }
    }
}