using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.OrderRepository
{
    public interface IOrderRepository :IGenericRepositoryAsync<Order>
    {
        
    }
}