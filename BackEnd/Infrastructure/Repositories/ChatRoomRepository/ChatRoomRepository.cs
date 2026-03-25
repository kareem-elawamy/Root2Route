using Domain.Models;
using Infrastructure.Base;
using Infrastructure.Data;

namespace Infrastructure.Repositories.ChatRoomRepository
{
    public class ChatRoomRepository : GenericRepositoryAsync<ChatRoom>, IChatRoomRepository
    {
        public ChatRoomRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
