using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Service.Services;

public interface IOrganizationService
{
    Task<string> CreateOrganizationAsync(Organization organization, IFormFile? imageFile = null);
    Task<bool> IsNameExistAsync(string name);
    // bool IsOrganizationTypeAllowed(UserType userType, OrganizationType orgType);
    // Task<List<Organization>> GetAllOnwerOrganizationsAsync(Guid ownerId);
}
