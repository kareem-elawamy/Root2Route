using Infrastructure.Base;
using Infrastructure.Repositories.CropRepository;
using Infrastructure.Repositories.FarmRepository;
using Infrastructure.Repositories.OrganizationMemberRepository;
using Infrastructure.Repositories.OrganizationRepository;
using Infrastructure.Repositories.OrganizationRoleRepository;
using Infrastructure.Repositories.PlantGuideStepRepository;
using Infrastructure.Repositories.PlantInfoRepository;
using Infrastructure.Repositories.ProductRepository;
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
        services.AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>();
        services.AddScoped<IOrganizationRoleRepository, OrganizationRoleRepository>();
        services.AddScoped<IFarmRepository, FarmRepository>();
        services.AddScoped<ICropRepository,CropRepository>();
        services.AddScoped<IProductRepository,ProductRepository>();
        return services;
    }
}
