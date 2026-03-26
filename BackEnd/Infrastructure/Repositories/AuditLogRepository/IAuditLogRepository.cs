using Domain.Models;
using Infrastructure.Base;

namespace Infrastructure.Repositories.AuditLogRepository
{
    public interface IAuditLogRepository : IGenericRepositoryAsync<AuditLog>
    {
    }
}
