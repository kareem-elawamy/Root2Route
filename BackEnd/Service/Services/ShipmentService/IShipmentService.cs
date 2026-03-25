using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;

namespace Service.Services.ShipmentService
{
    public interface IShipmentService
    {
        Task<Shipment> DispatchOrderAsync(Guid orderId, string carrierName, string trackingNumber, string? driverPhone, Guid currentUserId);
        Task<Shipment?> GetShipmentByOrderIdAsync(Guid orderId);
        Task UpdateShipmentStatusAsync(Guid shipmentId, ShipmentStatus newStatus, Guid currentUserId);
    }
}
