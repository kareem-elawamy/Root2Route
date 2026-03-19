using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Features.OrganizationMember.Commands.Models
{
    public record RemoveOrganizationMemberModel
    (
        Guid OrganizationMemberId,
        [property: System.Text.Json.Serialization.JsonIgnore] Guid CurrentUserId = default
    ) : IRequest<Response<string>>;
}