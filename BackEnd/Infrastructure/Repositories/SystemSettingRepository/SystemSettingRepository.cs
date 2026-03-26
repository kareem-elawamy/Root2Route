using Domain.Models;
using Infrastructure.Base;
using Infrastructure.Data;

namespace Infrastructure.Repositories.SystemSettingRepository
{
    public class SystemSettingRepository : GenericRepositoryAsync<SystemSetting>, ISystemSettingRepository
    {
        public SystemSettingRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
