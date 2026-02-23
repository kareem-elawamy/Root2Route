using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Core.Mapping.Authentication
{
    public partial class ApplicationUserProfile:Profile
    {
        public ApplicationUserProfile()
        {
            AddApplicationUserMapping();
        }
    }
}