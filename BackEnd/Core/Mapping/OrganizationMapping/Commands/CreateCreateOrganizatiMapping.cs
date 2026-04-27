using System;
using Core.Features.Organization.Commands.Models;

namespace Core.Mapping.OrganizationMapping
{
    public partial class OrganizationProfile
    {
        public void CreateOrganizationsMapping()
        {
            CreateMap<CreateOrganizationCommand, Organization>()
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
                .ForMember(dest => dest.LogoUrl, opt => opt.Ignore())
                .ForSourceMember(src => src.Logo, opt => opt.DoNotValidate())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));
        }
    }
}
