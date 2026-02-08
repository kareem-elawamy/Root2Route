using Microsoft.Extensions.DependencyInjection;
using Service.Services;
using Service.Services.AuthenticationService;
using Service.Services.AuthorizationService;
using Service.Services.CropService;
using Service.Services.FarmService;
using Service.Services.FileService;
using Service.Services.OrganizationMemberService;
using Service.Services.OrganizationRoleService;
using Service.Services.PlantGuideStepService;
using Service.Services.PlantInfoService;
using Service.Services.ProductService;

namespace Service;

public static class ModuleServiceDependencies
{
    public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
    {
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IPlantGuideStepService, PlantGuideStepService>();
        services.AddScoped<IPlantInfoService, PlantInfoService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<IOrganizationRoleService, OrganizationRoleService>();
        services.AddScoped<IOrganizationMemberService, OrganizationMemberService>();
        services.AddScoped<IFarmService, FarmService>();
        services.AddScoped<ICropService, CropService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }

}
