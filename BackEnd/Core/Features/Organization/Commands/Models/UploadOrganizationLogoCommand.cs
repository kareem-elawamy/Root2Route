using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Core.Features.Organization.Commands.Models
{
    public record UploadOrganizationLogoCommand(
     Guid OrganizationId,
     Guid OwnerId,
     IFormFile File
 ) : IRequest<Response<string>>;
}