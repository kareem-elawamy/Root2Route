using Domain.Enums;
using Infrastructure.Repositories.OrganizationRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Services.FileService;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IFileService _fileService;

    private readonly UserManager<ApplicationUser> _userManager;

    public OrganizationService(IOrganizationRepository organizationRepository, UserManager<ApplicationUser> userManager, IFileService fileService)
    {
        _fileService = fileService;
        _organizationRepository = organizationRepository;
        _userManager = userManager;
    }
    // 1. إنشاء منظمة جديدة 
    public async Task<string> CreateOrganizationAsync(Organization organization, IFormFile? imageFile = null)
    {
        // هنا ممكن تعمل أي Business Logic إضافي
        var user = await _userManager.FindByIdAsync(organization.OwnerId.ToString());

        if (user == null) return "User Not Found";

        if (imageFile != null)
        {
            var imageUrl = await _fileService.UploadImageAsync("Logo-images", imageFile);
            organization.LogoUrl = imageUrl;
        }
        var result = await _organizationRepository.AddAsync(organization);
        return result != null ? "Success" : "Failed";
    }
    // 2. جلب كل المنظمات التي يملكها مستخدم معين
    public async Task<List<Organization>> GetAllOnwerOrganizationsAsync(Guid ownerId)
    {
        var Organizations = await _organizationRepository.GetAllOrganizationsByOwnerId(ownerId);

        return Organizations;
    }
    // 3. جلب كل المنظمات التي يشارك فيها مستخدم معين (سواء كمالك أو كعضو)
    // public async Task<List<Organization>> GetAllOrganizationsByUserIdAsync(Guid userId)
    // {
    //     var Organizations = await _organizationRepository.GetAllOrganizationsByUserId(userId);

    //     return Organizations;
    // }

    public async Task<bool> IsNameExistAsync(string name)
    {
        var exists = await _organizationRepository.GetTableNoTracking()
                            .AnyAsync(x => x.Name == name);
        return exists;
    }
}