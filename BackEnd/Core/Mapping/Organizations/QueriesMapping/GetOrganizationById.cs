using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Organization.Queries.Models;

namespace Core.Mapping.Organizations
{
    public partial class OrganizationsProfile
    {
        void GetOrganizationById()
        {
            CreateMap<Organization,GetOrganizationsById>();
        }
    }
}