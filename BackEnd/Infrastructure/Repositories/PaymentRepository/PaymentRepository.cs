using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repositories.PaymentRepository
{
    public class PaymentRepository : GenericRepositoryAsync<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
