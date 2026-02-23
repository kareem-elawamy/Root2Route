using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Organization.Commands.Models;

namespace Core.Mapping.OrganizationMapping
{
    public partial class OrganizationProfile
    {
        public void UpdateOrganizationsMapping()
        {
            CreateMap<UpdateOrganizations, Domain.Models.Organization>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OrganizationId))
                .ForMember(dest => dest.LogoUrl, opt => opt.Ignore());
        }
    }
}