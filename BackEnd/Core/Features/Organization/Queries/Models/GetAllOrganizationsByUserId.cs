using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Organization.Queries.Result;

namespace Core.Features.Organization.Queries.Models
{
    public class GetAllOrganizationsByUserId : IRequest<Response<List<OrganizationResponse>>>
    {
        public Guid OwnerId { get; set; }

        public GetAllOrganizationsByUserId(Guid id)
        {
            OwnerId = id;
        }
    }
}