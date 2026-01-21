using Infrastructure.Abstract;
using Infrastructure.Base;
using Infrastructure.Repos;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ModuleInfrastructureDependencies
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
    {
        // Register infrastructure services here if needed
        services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
        services.AddScoped<IPlantInfoRepo, PlantInfoRepo>();
        return services;
    }
}
