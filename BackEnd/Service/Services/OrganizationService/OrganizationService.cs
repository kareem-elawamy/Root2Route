using Domain.Enums;
using Infrastructure.Repositories.OrganizationRepository;
using Infrastructure.Repositories.OrganizationMemberRepository;
using Infrastructure.Repositories.OrganizationRoleRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Services.FileService;
using Service.Services;
using Domain.Constants;

public class OrganizationService : IOrganizationService
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IOrganizationMemberRepository _memberRepository;
    private readonly IOrganizationRoleRepository _roleRepository;
    private readonly IFileService _fileService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    #region Con
    public OrganizationService(
        IOrganizationRepository organizationRepository,
        IOrganizationMemberRepository memberRepository,
        IOrganizationRoleRepository roleRepository,
        IFileService fileService,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _organizationRepository = organizationRepository;
        _memberRepository = memberRepository;
        _roleRepository = roleRepository;
        _fileService = fileService;
        _userManager = userManager;
        _context = context;
    }
    #endregion
    #region Create Organization

    public async Task<string> CreateOrganizationAsync(
Organization organization,
IFormFile? imageFile = null)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _userManager.FindByIdAsync(organization.OwnerId.ToString());
            if (user == null)
                return "Owner Not Found";

            var nameExists = await _organizationRepository
                .GetTableNoTracking()
                .AnyAsync(x => x.Name.ToLower() == organization.Name.ToLower());

            if (nameExists)
                return "Exists";

            if (imageFile != null)
                organization.LogoUrl = await _fileService.UploadImageAsync("Logo-images", imageFile);

            organization.Id = Guid.NewGuid();
            organization.CreatedAt = DateTime.UtcNow;
            organization.OrganizationStatus = OrganizationStatus.Approved;

            await _organizationRepository.AddAsync(organization);

            var ownerRole = new OrganizationRole
            {
                Id = Guid.NewGuid(),
                Name = "Owner",
                OrganizationId = organization.Id,
                IsSystemDefault = true,
                Permissions = OrganizationsPermissions.GetAll()
                    .Select(p => new OrganizationRolePermission
                    {
                        Id = Guid.NewGuid(),
                        PermissionsClaim = p
                    }).ToList()
            };

            await _roleRepository.AddAsync(ownerRole);

            var ownerMember = new OrganizationMember
            {
                Id = Guid.NewGuid(),
                OrganizationId = organization.Id,
                UserId = organization.OwnerId,
                OrganizationRoleId = ownerRole.Id,
                IsActive = true
            };

            await _memberRepository.AddAsync(ownerMember);

            await transaction.CommitAsync();

            return "Success";
        }
        catch
        {
            await transaction.RollbackAsync();
            return "Failed";
        }
    }
    #endregion

    #region Get Methods

    public async Task<Organization?> GetByIdAsync(Guid id)
    {
        return await _organizationRepository.GetByIdAsync(id);
    }

    public async Task<List<Organization>> GetAllAsync()
    {
        return await _organizationRepository
            .GetTableNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<Organization>> GetMyOrganizationsAsync(Guid userId)
    {
        return await _organizationRepository.GetAllOrganizationsByUserId(userId);
    }

    #endregion

    #region Update Organization

    public async Task<string> UpdateAsync(Guid id, Organization updatedData, IFormFile? newLogo = null)
    {
        var org = await _organizationRepository.GetByIdAsync(id);
        if (org == null || org.IsDeleted)
            return "Not Found";

        org.Name = updatedData.Name;
        org.Description = updatedData.Description;
        org.Address = updatedData.Address;
        org.ContactEmail = updatedData.ContactEmail;
        org.ContactPhone = updatedData.ContactPhone;

        // Fix: Include the missing Type property
        // org.Type = updatedData.Type; // Uncomment this if 'Type' exists on your Domain.Models.Organization

        // Fix: Handle the Logo upload if a new file was provided
        if (newLogo != null)
        {
            org.LogoUrl = await _fileService.UploadImageAsync("Logo-images", newLogo);
        }

        org.UpdatedAt = DateTime.UtcNow;

        await _organizationRepository.UpdateAsync(org);

        return "Success";
    }

    #endregion
    #region Soft Delete

    public async Task<string> SoftDeleteAsync(Guid id)
    {

        var org = await _organizationRepository.GetByIdAsync(id);
        if (org == null)
            return "Not Found";

        org.IsDeleted = true;
        org.DeleteAt = DateTime.UtcNow;
        org.OrganizationStatus = OrganizationStatus.Suspended;

        await _organizationRepository.UpdateAsync(org);

        return "Deleted";
    }

    public async Task<string> RestoreAsync(Guid id)
    {
        var org = await _organizationRepository.GetByIdAsync(id);
        if (org == null)
            return "Not Found";

        org.IsDeleted = false;
        org.OrganizationStatus = OrganizationStatus.Approved;

        await _organizationRepository.UpdateAsync(org);

        return "Restored";
    }

    #endregion

    #region Change Owner

    public async Task<string> ChangeOwnerAsync(Guid organizationId, Guid newOwnerId)
    {
        var org = await _organizationRepository.GetByIdAsync(organizationId);
        if (org == null)
            return "Not Found";

        var user = await _userManager.FindByIdAsync(newOwnerId.ToString());
        if (user == null)
            return "User Not Found";

        org.OwnerId = newOwnerId;
        await _organizationRepository.UpdateAsync(org);

        return "Owner Updated";
    }

    #endregion

    #region Update Status

    public async Task<string> UpdateStatusAsync(Guid organizationId, OrganizationStatus newStatus)
    {
        var org = await _organizationRepository.GetByIdAsync(organizationId);
        if (org == null)
            return "Not Found";

        if (org.OrganizationStatus == OrganizationStatus.Suspended &&
            newStatus == OrganizationStatus.Pending)
            return "Invalid Status Transition";

        org.OrganizationStatus = newStatus;
        await _organizationRepository.UpdateAsync(org);

        return "Status Updated";
    }

    #endregion

    #region Upload Logo

    public async Task<string> UploadLogoAsync(Guid organizationId, IFormFile file)
    {
        var org = await _organizationRepository.GetByIdAsync(organizationId);
        if (org == null)
            return "Not Found";

        org.LogoUrl = await _fileService.UploadImageAsync("Logo-images", file);
        await _organizationRepository.UpdateAsync(org);

        return "Logo Updated";
    }

    #endregion

    #region Statistics

    public async Task<object> GetStatisticsAsync(Guid organizationId)
    {
        var membersCount = await _memberRepository
            .GetTableNoTracking()
            .CountAsync(x => x.OrganizationId == organizationId);

        var marketItemsCount = await _context.MarketItems
            .CountAsync(x => x.OrganizationId == organizationId);

        return new
        {
            MembersCount = membersCount,
            MarketItemsCount = marketItemsCount
        };
    }

    #endregion
    public async Task<bool> IsOwnerAsync(Guid ownerId, Guid organizationId)
    {
        var organization = await _organizationRepository.GetByIdAsync(organizationId);

        if (organization == null || organization.IsDeleted)
            return false;

        return organization.OwnerId == ownerId;
    }

}