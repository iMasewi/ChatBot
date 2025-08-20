using Chat.DTOS;

namespace Chat.Service.Interfaces
{
    public interface IConversationsService
    {
        Task<IEnumerable<ConversationsDTO>> GetAllConversationsAsync();
        Task<IEnumerable<ConversationsDTO>> GetConversationsByUserIdAsync(int userId);
        Task<ConversationsDTO> GetConversationByIdAsync(int id);
        Task<ConversationsDTO> AddConversationAsync(ConversationsDTO conversationsDTO);
        Task DeleteConversationAsync(int id);
        Task<int> CountConversationsAsync(int userId);
    }
}
