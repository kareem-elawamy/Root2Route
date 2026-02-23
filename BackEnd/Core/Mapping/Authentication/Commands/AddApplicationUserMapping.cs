using AutoMapper;
using Core.Features.authentication.Commands.Models;
using Domain.Models;

namespace Core.Mapping.Authentication
{
    public partial class ApplicationUserProfile : Profile
    {
        public void AddApplicationUserMapping()
        {
            CreateMap<AddUserCommand, ApplicationUser>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
        }
    }
}