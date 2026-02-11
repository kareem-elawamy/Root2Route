using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.CropActivityLogRepository
{
    public class CropActivityLogRepository : GenericRepositoryAsync<CropActivityLog>, ICropActivityLogRepository
    {
        public CropActivityLogRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}