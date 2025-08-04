using Chat.Models;
using Chat.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chat.Repository
{
    public class ConversationsRepository : Repository<Conversations>, IConversationsRepository
    {
        public ConversationsRepository(Data.ApplicationDbContext context) : base(context)
        {
        }

        public async Task<int> CountConversationAsync(int userId)
        {
            return await _context.Conversations
                .CountAsync(c => c.UserId == userId);
        }

        public async Task<IEnumerable<Conversations>> GetConversationsByUserIdAsync(int userId)
        {
            return await _context.Conversations
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }
    }
}
