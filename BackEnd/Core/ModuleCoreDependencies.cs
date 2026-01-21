using System.ComponentModel.Design;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace Core;

public static class ModuleCoreDependencies
{
    public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
    {
        // Register infrastructure services here if needed
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
        
        return services;
    }
}
