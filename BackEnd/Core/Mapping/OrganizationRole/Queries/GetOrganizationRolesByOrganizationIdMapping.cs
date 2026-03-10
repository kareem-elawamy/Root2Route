using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.OrganizationRole.Queries.Result;

namespace Core.Mapping.OrganizationRole
{
    public partial class OrganizationRoleProfile
    {
        public void GetOrganizationRolesByOrganizationIdModelToGetOrganizationRolesByOrganizationIdResult()
        {
            CreateMap<Domain.Models.OrganizationRole, GetOrganizationRolesByOrganizationIdResult>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.IsSystemDefault, opt => opt.MapFrom(src => src.IsSystemDefault))
                .ForMember(dest => dest.OrganizationId, opt => opt.MapFrom(src => src.OrganizationId));
            CreateMap<Domain.Models.OrganizationRolePermission, OrganizationRolePermissionsList>();
        }
    }
}