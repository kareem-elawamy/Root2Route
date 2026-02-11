using Infrastructure.Base;
using Infrastructure.Repositories.AuctionRepository;
using Infrastructure.Repositories.BidRepository;
using Infrastructure.Repositories.ChatMessageRepository;
using Infrastructure.Repositories.ConversationRepository;
using Infrastructure.Repositories.CropActivityLogRepository;
using Infrastructure.Repositories.CropRepository;
using Infrastructure.Repositories.FarmRepository;
using Infrastructure.Repositories.OrderItemRepository;
using Infrastructure.Repositories.OrderRepository;
using Infrastructure.Repositories.OrganizationInvitationRepository;
using Infrastructure.Repositories.OrganizationMemberRepository;
using Infrastructure.Repositories.OrganizationRepository;
using Infrastructure.Repositories.OrganizationRoleRepository;
using Infrastructure.Repositories.PlantGuideStepRepository;
using Infrastructure.Repositories.PlantInfoRepository;
using Infrastructure.Repositories.ProductRepository;
using Infrastructure.Repositories.ReviewRepository;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ModuleInfrastructureDependencies
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
    {
        // Register infrastructure services here if needed
        services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
        services.AddScoped<IAuctionRepository, AuctionRepository>();
        services.AddScoped<IBidRepository, BidRepository>();
        services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<ICropActivityLogRepository, CropActivityLogRepository>();
        services.AddScoped<ICropRepository, CropRepository>();
        services.AddScoped<IFarmRepository, FarmRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrganizationInvitationRepository, OrganizationInvitationRepository>();
        services.AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IOrganizationRoleRepository, OrganizationRoleRepository>();

        services.AddScoped<IPlantGuideStepRepository, PlantGuideStepRepository>();
        services.AddScoped<IPlantInfoRepository, PlantInfoRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        return services;
    }
}
