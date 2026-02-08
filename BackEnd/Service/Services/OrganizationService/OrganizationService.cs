using Domain.Enums;
using Infrastructure.Repositories.OrganizationRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _organizationRepository;

    private readonly UserManager<ApplicationUser> _userManager;

    public OrganizationService(IOrganizationRepository organizationRepository, UserManager<ApplicationUser> userManager)
    {
        _organizationRepository = organizationRepository;
        _userManager = userManager;
    }
    public async Task<string> CreateOrganizationAsync(Organization organization)
    {
        // هنا ممكن تعمل أي Business Logic إضافي
        var user = await _userManager.FindByIdAsync(organization.OwnerId.ToString());

        if (user == null) return "User Not Found";
        if (!IsOrganizationTypeAllowed(user.UserType, organization.Type))
        {
            return $"User of type {user.UserType} cannot create an organization of type {organization.Type}.";
        }
        var existingOrg = await _organizationRepository.GetTableNoTracking()
                                .AnyAsync(x => x.OwnerId == organization.OwnerId);
        if (existingOrg) return "User already has an organization.";

        var result = await _organizationRepository.AddAsync(organization);
        return result != null ? "Success" : "Failed";
    }
    // دالة مساعدة لتنظيم القواعد (Business Rules)
    public bool IsOrganizationTypeAllowed(UserType userType, OrganizationType orgType)
    {
        switch (userType)
        {
            case UserType.Farmer:
                // المزارع لازم يعمل مزرعة فقط
                return orgType == OrganizationType.Farm;

            case UserType.Trader:
                // التاجر لازم يعمل متجر فقط
                return orgType == OrganizationType.Store;

            case UserType.BusinessOwner:
                // صاحب العمل ممكن يعمل مطعم، فندق، أو مصنع
                return orgType == OrganizationType.Restaurant ||
                       orgType == OrganizationType.Hotel ||
                       orgType == OrganizationType.Factory;

            case UserType.Admin:
                return true; // الأدمن يقدر يعمل أي حاجة

            default:
                return false; // أي نوع تاني ممنوع يعمل مؤسسة
        }
    }

    // public async Task<List<Organization>> GetAllOnwerOrganizationsAsync(Guid ownerId)
    // {
    //     var Organizations = await _organizationRepository.GetAllOrganizationsByOwnerId(ownerId);

    //     return Organizations;
    // }

    public async Task<bool> IsNameExistAsync(string name)
    {
        var exists = await _organizationRepository.GetTableNoTracking()
                            .AnyAsync(x => x.Name == name);
        return exists;
    }
}