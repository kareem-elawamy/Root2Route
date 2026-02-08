using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Organization.Queries.Result;
using MediatR;

namespace Core.Features.Organization.Queries.Models
{
    public class GetAllOwnerOrganizations : IRequest<Response<List<OrganizationResponse>>>
    {
        public Guid OwnerId { get; set; }
        public GetAllOwnerOrganizations(Guid ownerId)
        {
            OwnerId = ownerId;
        }
    }
}