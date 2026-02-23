using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.ConversationRepository
{
    public class ConversationRepository : GenericRepositoryAsync<Conversation>, IConversationRepository
    {
        public ConversationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}