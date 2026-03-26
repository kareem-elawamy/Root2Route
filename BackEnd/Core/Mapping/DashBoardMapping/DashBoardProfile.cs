using AutoMapper;
using Domain.Models;
using Service.DTOs;
using Service.DTOs.DashBoardDto;

namespace Core.Mapping.DashBoardMapping
{
    public class DashBoardProfile : Profile
    {
        public DashBoardProfile()
        {
            // Pending Organization Mapping
            CreateMap<Organization, PendingOrganizationDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.RequestDate, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
}
