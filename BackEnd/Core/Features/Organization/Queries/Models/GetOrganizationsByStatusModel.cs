using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Organization.Queries.Result;
using Domain.Enums;

namespace Core.Features.Organization.Queries.Models
{
    public class GetOrganizationsByStatusModel : IRequest<Response<List<OrganizationResponse>>>

    {
        public OrganizationStatus Status { get; set; }

        public GetOrganizationsByStatusModel(OrganizationStatus status)
        {
            Status = status;
        }
        
    }
}