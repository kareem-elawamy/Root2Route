using System;
using Core.Base;
using MediatR;

namespace Core.Features.Organization.Commands.Models
{
    public class ApproveOrganizationCommand : IRequest<Response<string>>
    {
        public Guid OrganizationId { get; set; }
        public Guid AdminId { get; set; }
    }

    public class RejectOrganizationCommand : IRequest<Response<string>>
    {
        public Guid OrganizationId { get; set; }
        public Guid AdminId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
