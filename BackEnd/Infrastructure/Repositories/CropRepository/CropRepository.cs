using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.CropRepository
{
    public class CropRepository : GenericRepositoryAsync<Crop>, ICropRepository
    {
        public CropRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
            
    }
}