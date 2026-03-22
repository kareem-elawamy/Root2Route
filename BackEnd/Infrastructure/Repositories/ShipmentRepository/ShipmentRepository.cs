using Domain.Models;
using Infrastructure.Base;
using Infrastructure.Data;

namespace Infrastructure.Repositories.ShipmentRepository
{
    public class ShipmentRepository : GenericRepositoryAsync<Shipment>, IShipmentRepository
    {
        public ShipmentRepository(ApplicationDbContext context) : base(context) { }
    }
}
