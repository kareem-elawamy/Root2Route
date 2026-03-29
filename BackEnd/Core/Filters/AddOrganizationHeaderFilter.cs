using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Filters
{
    /// <summary>
    /// Shows the X-Organization-Id header ONLY on endpoints that actually read it:
    ///   1. Endpoints decorated with [HasPermission] (uses PermissionAuthorizationFilter internally)
    ///   2. Endpoints with [FromHeader(Name = "X-Organization-Id")] parameter
    /// </summary>
    public class AddOrganizationHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var methodInfo = context.MethodInfo;

            // Check 1: Does this action or its controller have [HasPermission] attribute?
            bool hasPermissionAttribute =
                methodInfo.GetCustomAttributes(true).Any(a => a.GetType().Name == "HasPermissionAttribute") ||
                (methodInfo.DeclaringType?.GetCustomAttributes(true).Any(a => a.GetType().Name == "HasPermissionAttribute") ?? false);

            // Check 2: Does this action already have [FromHeader("X-Organization-Id")] as a parameter?
            // If so, Swagger will auto-add a parameter for it via model binding — no need to duplicate.
            bool hasFromHeaderParam = methodInfo.GetParameters()
                .Any(p => p.GetCustomAttributes(true)
                    .OfType<Microsoft.AspNetCore.Mvc.FromHeaderAttribute>()
                    .Any(h => h.Name == "X-Organization-Id"));

            // Only add the header if [HasPermission] is present AND it's not already bound via [FromHeader]
            if (!hasPermissionAttribute || hasFromHeaderParam)
                return;

            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Organization-Id",
                In = ParameterLocation.Header,
                Required = true, // If [HasPermission] is on the endpoint, the header IS required
                Description = "The Organization ID (Guid) — required for permission-checked endpoints",
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Format = "uuid"
                }
            });
        }
    }
}