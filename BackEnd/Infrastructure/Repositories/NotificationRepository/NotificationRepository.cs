using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repositories.NotificationRepository
{
    public class NotificationRepository : GenericRepositoryAsync<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
