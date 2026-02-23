using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.ChatMessageRepository
{
    public class ChatMessageRepository : GenericRepositoryAsync<ChatMessage>, IChatMessageRepository
    {
        public ChatMessageRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}