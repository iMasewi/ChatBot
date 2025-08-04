using Chat.Models;

namespace Chat.Repository.Interfaces
{
    public interface IConversationsRepository : IRepository<Conversations>
    {
        Task<IEnumerable<Conversations>> GetConversationsByUserIdAsync(int userId);
        Task<int> CountConversationAsync(int userId);
    }
}
