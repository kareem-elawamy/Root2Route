using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repositories.OrderStatusHistoryRepository
{
    public class OrderStatusHistoryRepository : GenericRepositoryAsync<OrderStatusHistory>, IOrderStatusHistoryRepository
    {
        public OrderStatusHistoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
