using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Core.Features.Organization.Commands.Models
{
    public class UpdateOrganizations : IRequest<Response<string>>
    {
        [BindNever]
        public Guid OwnerId { get; set; }

        public Guid OrganizationId { get; set; }

        public UpdateOrganizations() { }

        public UpdateOrganizations(Guid ownerId, Guid organizationId)
        {
            OwnerId = ownerId;
            OrganizationId = organizationId;
        }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public FormFile? Logo { get; set; }
        public OrganizationType Type { get; set; }
    }
}