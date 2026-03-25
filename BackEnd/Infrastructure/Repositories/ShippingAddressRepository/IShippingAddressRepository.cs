using Infrastructure.Base;
using Domain.Models;

namespace Infrastructure.Repositories.ShippingAddressRepository
{
    public interface IShippingAddressRepository : IGenericRepositoryAsync<ShippingAddress>
    {
    }
}
