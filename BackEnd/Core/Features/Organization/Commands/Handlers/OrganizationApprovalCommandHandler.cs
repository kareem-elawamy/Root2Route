using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Organization.Commands.Models;
using Domain.Enums;
using Infrastructure.Repositories.OrganizationRepository;
using MediatR;

namespace Core.Features.Organization.Commands.Handlers
{
    public class OrganizationApprovalCommandHandler : ResponseHandler,
        IRequestHandler<ApproveOrganizationCommand, Response<string>>,
        IRequestHandler<RejectOrganizationCommand, Response<string>>
    {
        private readonly IOrganizationRepository _orgRepo;

        public OrganizationApprovalCommandHandler(IOrganizationRepository orgRepo)
        {
            _orgRepo = orgRepo;
        }

        public async Task<Response<string>> Handle(ApproveOrganizationCommand request, CancellationToken cancellationToken)
        {
            var org = await _orgRepo.GetByIdAsync(request.OrganizationId);
            if (org == null) return NotFound<string>("Organization not found.");

            if (org.OrganizationStatus == OrganizationStatus.Approved)
                return BadRequest<string>("Organization is already approved.");

            org.OrganizationStatus = OrganizationStatus.Approved;
            org.UpdatedAt = System.DateTime.UtcNow;

            await _orgRepo.UpdateAsync(org);
            return Success("Organization Approved Successfully.");
        }

        public async Task<Response<string>> Handle(RejectOrganizationCommand request, CancellationToken cancellationToken)
        {
            var org = await _orgRepo.GetByIdAsync(request.OrganizationId);
            if (org == null) return NotFound<string>("Organization not found.");

            if (org.OrganizationStatus == OrganizationStatus.Approved)
                return BadRequest<string>("Cannot reject an already approved organization.");

            org.OrganizationStatus = OrganizationStatus.Rejected;
            org.UpdatedAt = System.DateTime.UtcNow;

            await _orgRepo.UpdateAsync(org);
            return Success($"Organization Rejected. Reason: {request.Reason}");
        }
    }
}
