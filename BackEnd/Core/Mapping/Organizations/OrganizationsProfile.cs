using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Mapping.Organizations
{
    public partial class OrganizationsProfile : Profile
    {
        public OrganizationsProfile()
        {

            GetOrganizationById();
            UpdateOrganization();

        }
    }
}