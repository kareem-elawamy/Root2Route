using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Services;

public interface IOrganizationService
{
    Task<string> CreateOrganizationAsync(Organization organization);
    Task<bool> IsNameExistAsync(string name);
}
