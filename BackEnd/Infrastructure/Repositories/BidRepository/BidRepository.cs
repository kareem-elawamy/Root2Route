using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.BidRepository
{
    public class BidRepository : GenericRepositoryAsync<Bid>, IBidRepository
    {
        public BidRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}