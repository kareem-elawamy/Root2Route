using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Core.Mapping.OrganizationMapping
{
    public partial class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            GetAllOnwerOrganizationsMapping();
        }
    }
}