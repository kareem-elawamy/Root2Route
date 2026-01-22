using Infrastructure.Repositories.OrganizationRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _organizationRepository;

    public OrganizationService(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<string> CreateOrganizationAsync(Organization organization)
    {
        // هنا ممكن تعمل أي Business Logic إضافي
        var result = await _organizationRepository.AddAsync(organization);

        return result != null ? "Success" : "Failed";
    }

    public async Task<bool> IsNameExistAsync(string name)
    {
        var exists = await _organizationRepository.GetTableNoTracking()
                            .AnyAsync(x => x.Name == name);
        return exists;
    }
}