using Chat.Models;
using Chat.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chat.Repository
{
    public class MessagesRepository : Repository<Messages>, IMessagesRepository
    {
        public MessagesRepository(Data.ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Messages>> GetMessagesByConversationIdAsync(int conversationId)
        {
            return await _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .ToListAsync();
        }
    }
}
