using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.OrganizationRole.Commands.Models;

namespace Core.Mapping.OrganizationRole
{
    public partial class OrganizationRoleProfile
    {
        public void CreateOrganizationRole()
        {
            CreateMap<AddOrganizationRoleCommand, Domain.Models.OrganizationRole>();
        }
    }
}