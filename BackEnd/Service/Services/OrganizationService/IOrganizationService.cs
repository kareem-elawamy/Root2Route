using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Service.Services;

public interface IOrganizationService
{
    Task<string> CreateOrganizationAsync(Organization organization, IFormFile? imageFile = null);
    Task<List<Organization>> GetAllAsync();
    Task<Organization?> GetByIdAsync(Guid id);
    Task<List<Organization>> GetMyOrganizationsAsync(Guid userId);
    Task<string> UpdateAsync(Guid id, Organization updatedData, IFormFile? newLogo = null);
    Task<string> UpdateStatusAsync(Guid organizationId, OrganizationStatus newStatus);
    Task<string> SoftDeleteAsync(Guid id, Guid currentUserId);
    Task<string> RestoreAsync(Guid id);
    Task<string> UploadLogoAsync(Guid organizationId, IFormFile file);
    Task<object> GetStatisticsAsync(Guid organizationId);
    Task<string> ChangeOwnerAsync(Guid organizationId, string email, Guid currentOwnerId);
    Task<bool> IsOwnerAsync(Guid ownerId, Guid organizationId);
    Task<List<Organization>> GetOrganizationsByStatusAsync(OrganizationStatus status);
    Task<Guid?> GetFirstOrganizationIdForUserAsync(Guid userId);


}
