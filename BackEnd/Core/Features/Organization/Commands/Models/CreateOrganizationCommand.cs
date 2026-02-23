using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Core.Features.Organization.Commands.Models
{
    public class CreateOrganizationCommand : IRequest<Response<string>>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public FormFile? Logo { get; set; }
        public OrganizationType Type { get; set; }
        public Guid OwnerId { get; set; }

    }
}