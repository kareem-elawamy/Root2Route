using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.OrganizationMember.Queries.Model;
using Core.Features.OrganizationMember.Queries.Result;

namespace Core.Mapping.OrganizationMembersMapping
{
    public partial class OrganizationMembersProfile
    {
        public void GetOrganizationMembersByOrganizationIdMapping()
        {
            CreateMap<OrganizationMember, GetOrganizationMembersByOrganizationIdResult>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User!.UserName))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.OrganizationRoles.Select(r => r.Name).ToList()))
                .ForMember(dest => dest.JoinedAt, opt => opt.MapFrom(src => src.JoinedAt));

        }
    }
}