using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Infrastructure.Base;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.PlantInfoRepository
{
    public class PlantInfoRepository : GenericRepositoryAsync<PlantInfo>, IPlantInfoRepository
    {
        private readonly DbSet<PlantInfo> _plantInfos;
        public PlantInfoRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _plantInfos = dbContext.Set<PlantInfo>();
        }

        public async Task<List<PlantInfo>> GetAllPlantInfosAsync()
        {
            return await _plantInfos.ToListAsync();
        }
    }
}