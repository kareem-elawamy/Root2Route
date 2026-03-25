using Infrastructure.Base;
using Infrastructure.Repositories.AuctionRepository;
using Infrastructure.Repositories.BidRepository;
using Infrastructure.Repositories.ChatMessageRepository;
using Infrastructure.Repositories.ChatRoomRepository;
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
using Infrastructure.Repositories.NotificationRepository;
using Infrastructure.Repositories.PaymentRepository;
using Infrastructure.Repositories.OrderStatusHistoryRepository;
using Infrastructure.Repositories.ShippingAddressRepository;
using Infrastructure.Repositories.ShipmentRepository;
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
        services.AddScoped<IChatRoomRepository, ChatRoomRepository>();

        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrganizationInvitationRepository, OrganizationInvitationRepository>();
        services.AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IOrganizationRoleRepository, OrganizationRoleRepository>();

        services.AddScoped<IPlantGuideStepRepository, PlantGuideStepRepository>();
        services.AddScoped<IPlantInfoRepository, PlantInfoRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IChatRoomRepository, ChatRoomRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();
        services.AddScoped<IShippingAddressRepository, ShippingAddressRepository>();
        services.AddScoped<IShipmentRepository, ShipmentRepository>();
        return services;
    }
}
