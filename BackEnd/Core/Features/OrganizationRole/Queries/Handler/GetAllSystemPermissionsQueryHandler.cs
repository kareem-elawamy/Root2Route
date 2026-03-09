using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.OrganizationRole.Queries.Models;
using Domain.Constants;

namespace Core.Features.OrganizationRole.Queries.Handler
{
    public class GetAllSystemPermissionsQueryHandler : ResponseHandler
    , IRequestHandler<GetAllSystemPermissionsQuery, Response<List<string>>>
    {
        public async Task<Response<List<string>>> Handle(GetAllSystemPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = Permissions.GetAllPermissions();
            return Success(permissions);
        }
    }
}