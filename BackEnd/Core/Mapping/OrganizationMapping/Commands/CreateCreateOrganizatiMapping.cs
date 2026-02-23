using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Organization.Commands.Models;

namespace Core.Mapping.OrganizationMapping
{
    public partial class OrganizationProfile
    {

        public void CreateOrganizationsMapping()
        {
            CreateMap<CreateOrganizationCommand, Organization>().ForMember(dest => dest.LogoUrl, opt => opt.Ignore());
        }
    }
}