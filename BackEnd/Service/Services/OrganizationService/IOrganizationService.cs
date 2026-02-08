using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

namespace Service.Services;

public interface IOrganizationService
{
    Task<string> CreateOrganizationAsync(Organization organization);
    Task<bool> IsNameExistAsync(string name);
    bool IsOrganizationTypeAllowed(UserType userType, OrganizationType orgType);
    // Task<List<Organization>> GetAllOnwerOrganizationsAsync(Guid ownerId);
}
