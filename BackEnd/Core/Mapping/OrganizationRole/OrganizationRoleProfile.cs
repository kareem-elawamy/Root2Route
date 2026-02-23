using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Core.Mapping.OrganizationRole
{
    public partial class OrganizationRoleProfile : Profile
    {
        public OrganizationRoleProfile()
        {
            CreateOrganizationRole();
            
        }
    }
}