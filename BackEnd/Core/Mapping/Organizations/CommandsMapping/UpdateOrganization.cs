using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Organization.Commands.Models;

namespace Core.Mapping.Organizations
{
    public partial class OrganizationsProfile
    {
        public void UpdateOrganization()
        {
            CreateMap<UpdateOrganizations, Organization>().
            ForMember(dest => dest.LogoUrl, otp => otp.Ignore());
        }
    }
}