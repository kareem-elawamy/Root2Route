using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.AuctionRepository
{
    public class AuctionRepository : GenericRepositoryAsync<Auction>, IAuctionRepository
    {
        public AuctionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}