using System;
using Microsoft.AspNetCore.Http;

namespace Core.Features.Organization.Commands.Models
{
    public class CreateOrganizationCommand : IRequest<Response<string>>
    {
        public Guid OwnerId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        public IFormFile? Logo { get; set; }
        public OrganizationType Type { get; set; }

    }
}