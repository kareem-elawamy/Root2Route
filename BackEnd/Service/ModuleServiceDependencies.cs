using Microsoft.Extensions.DependencyInjection;
using Service.Services;
using Service.Services.AuthenticationService;
using Service.Services.FileService;
using Service.Services.PlantGuideStepService;
using Service.Services.PlantInfoService;

namespace Service;

public static class ModuleServiceDependencies
{
    public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
    {
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IPlantGuideStepService, PlantGuideStepService>();
        services.AddScoped<IPlantInfoService, PlantInfoService>();
        services.AddScoped<IOrganizationService,OrganizationService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        return services;
    }

}
