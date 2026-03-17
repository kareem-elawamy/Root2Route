using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Mapping.Organizations
{
    public partial class OrganizationsProfile
    {

        public void GetAllOrganizationMapping()
        {
            CreateMap<Domain.Models.Organization, Core.Features.Organization.Queries.Result.OrganizationResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
                .ForMember(dest => dest.OrganizationStatus, opt => opt.MapFrom(src => src.OrganizationStatus));



        }
    }
}