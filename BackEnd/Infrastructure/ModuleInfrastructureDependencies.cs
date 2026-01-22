using Infrastructure.Base;
using Infrastructure.Repositories.OrganizationRepository;
using Infrastructure.Repositories.PlantGuideStepRepository;
using Infrastructure.Repositories.PlantInfoRepository;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ModuleInfrastructureDependencies
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
    {
        // Register infrastructure services here if needed
        services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
        services.AddScoped<IPlantInfoRepository, PlantInfoRepository>();
        services.AddScoped<IPlantGuideStepRepository, PlantGuideStepRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        return services;
    }
}
