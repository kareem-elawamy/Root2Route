using System;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories.OrderRepository;
using Infrastructure.Repositories.OrganizationMemberRepository;
using Infrastructure.Repositories.ShipmentRepository;
using Microsoft.EntityFrameworkCore;
using Service.Services.OrderService;

namespace Service.Services.ShipmentService
{
    public class ShipmentService : IShipmentService
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderService _orderService;
        private readonly IOrganizationMemberRepository _orgMemberRepo;

        public ShipmentService(
            IShipmentRepository shipmentRepository,
            IOrderRepository orderRepository,
            IOrderService orderService,
            IOrganizationMemberRepository orgMemberRepo)
        {
            _shipmentRepository = shipmentRepository;
            _orderRepository = orderRepository;
            _orderService = orderService;
            _orgMemberRepo = orgMemberRepo;
        }

        public async Task<Shipment> DispatchOrderAsync(Guid orderId, string carrierName, string trackingNumber, string? driverPhone, Guid currentUserId)
        {
            var order = await _orderRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            if (order.Status != OrderStatus.Confirmed)
                throw new InvalidOperationException($"Order must be in Confirmed status to dispatch. Current status: {order.Status}");

            // Verify seller authorization
            bool isSeller = await _orgMemberRepo.GetTableNoTracking()
                .AnyAsync(m => m.OrganizationId == order.OrganizationId && m.UserId == currentUserId && m.IsActive);

            if (!isSeller)
                throw new UnauthorizedAccessException("Only active members of the selling organization can dispatch this order.");

            // Check if shipment already exists
            var existingShipment = await _shipmentRepository.GetTableNoTracking()
                .AnyAsync(s => s.OrderId == orderId);

            if (existingShipment)
                throw new InvalidOperationException("A shipment already exists for this order.");

            var shipment = new Shipment
            {
                OrderId = orderId,
                CarrierName = carrierName,
                TrackingNumber = trackingNumber,
                DriverPhone = driverPhone,
                Status = ShipmentStatus.Pending
            };

            await _shipmentRepository.AddAsync(shipment);
            await _shipmentRepository.SaveChangesAsync();

            // Transition order to Shipped via the state machine
            await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Shipped, currentUserId, $"Dispatched via {carrierName}, tracking: {trackingNumber}");

            return shipment;
        }

        public async Task<Shipment?> GetShipmentByOrderIdAsync(Guid orderId)
        {
            return await _shipmentRepository.GetTableNoTracking()
                .Include(s => s.Order)
                .FirstOrDefaultAsync(s => s.OrderId == orderId);
        }

        public async Task UpdateShipmentStatusAsync(Guid shipmentId, ShipmentStatus newStatus, Guid currentUserId)
        {
            var shipment = await _shipmentRepository.GetTableAsTracking()
                .Include(s => s.Order)
                .FirstOrDefaultAsync(s => s.Id == shipmentId);

            if (shipment == null)
                throw new KeyNotFoundException("Shipment not found.");

            // Verify seller authorization
            bool isSeller = await _orgMemberRepo.GetTableNoTracking()
                .AnyAsync(m => m.OrganizationId == shipment.Order!.OrganizationId && m.UserId == currentUserId && m.IsActive);

            if (!isSeller)
                throw new UnauthorizedAccessException("Only active members of the selling organization can update this shipment.");

            shipment.Status = newStatus;

            await _shipmentRepository.UpdateAsync(shipment);
            await _shipmentRepository.SaveChangesAsync();
        }
    }
}
