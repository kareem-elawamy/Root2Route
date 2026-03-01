using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Organization.Commands.Models;
using Domain.Constants;
using Infrastructure.Data;
using Infrastructure.Repositories.OrganizationRepository;
using Service.Services;
using Service.Services.FileService;
using Service.Services.OrganizationMemberService;
using Service.Services.OrganizationRoleService;

namespace Core.Features.Organization.Commands.Handler
{
    public class OrganizationHandler : ResponseHandler, IRequestHandler<CreateOrganizationCommand, Response<string>>,
    IRequestHandler<UpdateOrganizations, Response<string>>,
      IRequestHandler<SoftDeleteOrganizationCommand, Response<string>>,
      IRequestHandler<RestoreOrganizationCommand, Response<string>>,
      IRequestHandler<ChangeOwnerCommand, Response<string>>,
       IRequestHandler<UpdateOrganizationStatusCommand, Response<string>>,
       IRequestHandler<UploadOrganizationLogoCommand, Response<string>>

    {
        private readonly IOrganizationService _organizationService;
        private readonly IMapper _mapper;

        public OrganizationHandler(IOrganizationService organizationService, IMapper mapper)
        {

            _organizationService = organizationService;
            _mapper = mapper;

        }

        public async Task<Response<string>> Handle(
    CreateOrganizationCommand request,
    CancellationToken cancellationToken)
        {
            var organization = _mapper.Map<Domain.Models.Organization>(request);

            var result = await _organizationService
                .CreateOrganizationAsync(organization, request.Logo);

            if (result == "Exists")
                return BadRequest<string>("Organization already exists");

            if (result == "Owner Not Found")
                return BadRequest<string>("Owner not found");

            if (result == "Failed")
                return BadRequest<string>("Something went wrong");

            return Created("Organization created successfully");
        }
        public async Task<Response<string>> Handle(UpdateOrganizations request, CancellationToken cancellationToken)
        {
            var organization = await _organizationService.GetByIdAsync(request.OrganizationId);

            if (organization == null)
                return NotFound<string>("Organization not found");
            var isOwner = await _organizationService.IsOwnerAsync(request.OwnerId, request.OrganizationId);
            if (!isOwner)
                return Unauthorized<string>("You are not allowed to update this organization");

            var mappedOrganization = _mapper.Map<Domain.Models.Organization>(request);

            // Fix: Pass the Logo file from the request down to the service
            var result = await _organizationService.UpdateAsync(request.OrganizationId, mappedOrganization, request.Logo);

            if (result == "Not Found")
                return NotFound<string>("Organization not found");

            return Success("Organization updated successfully");
        }

        public async Task<Response<string>> Handle(SoftDeleteOrganizationCommand request, CancellationToken cancellationToken)
        {
            var isOwner = await _organizationService
            .IsOwnerAsync(request.OwnerId, request.OrganizationId);
            if (!isOwner)
                return Unauthorized<string>("You are not allowed to delete this organization");
            var result = await _organizationService
           .SoftDeleteAsync(request.OrganizationId);
            if (result == "Not Found")
                return NotFound<string>("Organization not found");
            return Success("Organization deleted successfully");
        }

        public async Task<Response<string>> Handle(RestoreOrganizationCommand request, CancellationToken cancellationToken)
        {
            var isOwner = await _organizationService
           .IsOwnerAsync(request.OwnerId, request.OrganizationId);

            if (!isOwner)
                return Unauthorized<string>("You are not allowed to restore this organization");

            var result = await _organizationService
                .RestoreAsync(request.OrganizationId);

            if (result == "Not Found")
                return NotFound<string>("Organization not found");

            return Success("Organization restored successfully");
        }

        public async Task<Response<string>> Handle(ChangeOwnerCommand request, CancellationToken cancellationToken)
        {
            var isOwner = await _organizationService
           .IsOwnerAsync(request.CurrentOwnerId, request.OrganizationId);
            if (!isOwner)
                return Unauthorized<string>("Only the current owner can change ownership");
            var result = await _organizationService
      .ChangeOwnerAsync(request.OrganizationId, request.NewOwnerId);
            if (result == "Not Found")
                return NotFound<string>("Organization not found");
            if (result == "User Not Found")
                return NotFound<string>("New owner not found");
            return Success("Ownership transferred successfully");

        }

        public async Task<Response<string>> Handle(UpdateOrganizationStatusCommand request, CancellationToken cancellationToken)
        {
            var result = await _organizationService
           .UpdateStatusAsync(request.OrganizationId, request.NewStatus);

            if (result == "Not Found")
                return NotFound<string>("Organization not found");

            if (result == "Invalid Status Transition")
                return BadRequest<string>("Invalid status transition");

            return Success("Status updated successfully");
        }

        public async Task<Response<string>> Handle(UploadOrganizationLogoCommand request, CancellationToken cancellationToken)
        {
            var isOwner = await _organizationService
                        .IsOwnerAsync(request.OwnerId, request.OrganizationId);

            if (!isOwner)
                return Unauthorized<string>("You are not allowed to upload logo");

            var result = await _organizationService
                .UploadLogoAsync(request.OrganizationId, request.File);

            if (result == "Not Found")
                return NotFound<string>("Organization not found");

            return Success("Logo updated successfully");
        }
    }
}