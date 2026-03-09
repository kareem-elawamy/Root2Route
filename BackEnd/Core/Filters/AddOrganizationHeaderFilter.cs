using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
namespace Core.Filters
{
    public class AddOrganizationHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Organization-Id",
                In = ParameterLocation.Header,
                Required = false, // خليناها false عشان مش كل الـ Endpoints هتحتاجه (زي الـ Login مثلاً)
                Description = "Enter the Organization ID (Guid) here",
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Format = "uuid" // عشان Swagger يعرف إنه Guid
                }
            });


        }
    }
}