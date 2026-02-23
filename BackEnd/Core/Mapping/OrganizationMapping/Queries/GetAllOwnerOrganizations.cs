using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Organization.Queries.Result;
using Domain.Models;

namespace Core.Mapping.OrganizationMapping
{
    public partial class OrganizationProfile
    {
        public void GetAllOnwerOrganizationsMapping()
        {
            CreateMap<Organization,OrganizationResponse>();
        }
    }
}