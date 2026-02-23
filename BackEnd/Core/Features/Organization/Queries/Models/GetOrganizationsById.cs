using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Organization.Queries.Result;
using Infrastructure.Repositories.OrganizationRepository;

namespace Core.Features.Organization.Queries.Models
{
    public class GetOrganizationsById : IRequest<Response<OrganizationResponse>>
    {
        public Guid Id { get; set; }

        public GetOrganizationsById(Guid id)
        {
            Id = id;
        }
    }
}