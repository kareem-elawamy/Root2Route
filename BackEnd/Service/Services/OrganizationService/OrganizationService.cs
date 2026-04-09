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

    public async Task<string> CreateOrganizationAsync(Organization organization, IFormFile? imageFile = null)
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
            organization.OrganizationStatus = OrganizationStatus.Pending;

            await _organizationRepository.AddAsync(organization);

            var ownerRole = new OrganizationRole
            {
                Id = Guid.NewGuid(),
                Name = "Owner",
                OrganizationId = organization.Id,
                IsSystemDefault = true,
                Permissions = Permissions.GetAllPermissions()
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
                OrganizationRoles = new List<OrganizationRole> { ownerRole },
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
        if (id == Guid.Empty)
            return null;
        if (IsOrganizationActiveAsync(id).Result == false)
            return null;
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
    public async Task<List<Organization>> GetAllPendingOrganizationsAsync()
    {
        return await _organizationRepository
            .GetTableNoTracking()
            .Where(x => x.OrganizationStatus == OrganizationStatus.Pending && !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<Organization>> GetOrganizationsByStatusAsync(OrganizationStatus status)
    {
        return await _organizationRepository
            .GetTableNoTracking()
            .Where(x => x.OrganizationStatus == status && !x.IsDeleted)
            .ToListAsync();
    }
    #endregion



    #region Update Organization

    public async Task<string> UpdateAsync(Guid id, Organization updatedData, IFormFile? newLogo = null)
    {
        var org = await _organizationRepository.GetByIdAsync(id);
        if (org == null || org.IsDeleted)
            return "Not Found";
        if (org.OwnerId != updatedData.OwnerId)
            return "Unauthorized: Only the owner can update the organization.";
        if (IsOrganizationActiveAsync(id).Result == false)
            return "Organization is not active";
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

    public async Task<string> SoftDeleteAsync(Guid id, Guid currentUserId)
    {

        var org = await _organizationRepository.GetByIdAsync(id);
        if (org == null)
            return "Not Found";
        if (org.OwnerId != currentUserId)
            return "Unauthorized: Only the owner can delete the organization.";

        org.IsDeleted = true;
        org.DeleteAt = DateTime.UtcNow;
        org.OrganizationStatus = OrganizationStatus.Suspended;

        var productsToSuspend = await _context.Products.Where(p => p.OrganizationId == id && !p.IsDeleted).ToListAsync();
        foreach (var product in productsToSuspend)
        {
            product.IsDeleted = true;
        }

        await _organizationRepository.UpdateAsync(org);
        await _context.SaveChangesAsync();

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

    public async Task<string> ChangeOwnerAsync(Guid organizationId, string email, Guid currentOwnerId)
    {
        var org = await _organizationRepository.GetByIdAsync(organizationId);
        if (org == null)
            return "Not Found";
        if (org.OwnerId != currentOwnerId)
            return "Unauthorized: Only the current owner can transfer ownership.";

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return "User Not Found";

        org.OwnerId = user.Id;
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

        var marketItemsCount = await _context.Products
            .CountAsync(x => x.OrganizationId == organizationId);

        return new
        {
            MembersCount = membersCount,
            MarketItemsCount = marketItemsCount
        };
    }

    #endregion
    #region IsOwnerAsync
    public async Task<bool> IsOwnerAsync(Guid ownerId, Guid organizationId)
    {
        var organization = await _organizationRepository.GetByIdAsync(organizationId);

        if (organization == null || organization.IsDeleted)
            return false;

        return organization.OwnerId == ownerId;
    }
    #endregion
    #region Check Organization Status 
    public async Task<bool> IsOrganizationActiveAsync(Guid organizationId)
    {
        var organization = await _organizationRepository.GetByIdAsync(organizationId);

        if (organization == null || organization.IsDeleted)
            return false;

        return organization.OrganizationStatus == OrganizationStatus.Approved;
    }
    #endregion
    #region Check Organization Existence
    public async Task<bool> DoesOrganizationExistAsync(Guid organizationId)
    {
        return await _organizationRepository.GetTableNoTracking()
            .AnyAsync(x => x.Id == organizationId && !x.IsDeleted);
    }

    public async Task<Guid?> GetFirstOrganizationIdForUserAsync(Guid userId)
    {
        var organizations = await _organizationRepository.GetAllOrganizationsByUserId(userId);
        return organizations.FirstOrDefault()?.Id;
    }
    #endregion

}