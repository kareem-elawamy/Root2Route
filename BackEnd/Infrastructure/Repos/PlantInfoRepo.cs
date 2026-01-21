using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Infrastructure.Abstract;
using Infrastructure.Base;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos
{
    public class PlantInfoRepo : GenericRepositoryAsync<PlantInfo>, IPlantInfoRepo
    {
        private readonly DbSet<PlantInfo> _plantInfos;
        public PlantInfoRepo(ApplicationDbContext dbContext) : base(dbContext)
        {
            _plantInfos = dbContext.Set<PlantInfo>();
        }

        public async Task<List<PlantInfo>> GetAllPlantInfosAsync()
        {
            return await _plantInfos.ToListAsync();
        }
    }
}