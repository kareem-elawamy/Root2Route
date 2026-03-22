using Domain.Models;
using Infrastructure.Base;
using Infrastructure.Data;

namespace Infrastructure.Repositories.ShippingAddressRepository
{
    public class ShippingAddressRepository : GenericRepositoryAsync<ShippingAddress>, IShippingAddressRepository
    {
        public ShippingAddressRepository(ApplicationDbContext context) : base(context) { }
    }
}
