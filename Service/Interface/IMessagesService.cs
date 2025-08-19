using Chat.DTOS;

namespace Chat.Service.Interfaces
{
    public interface IMessagesService
    {
        Task<IEnumerable<MessagesDTO>> GetMessagesByConversationIdAsync(int conversationId);
        Task<MessagesDTO> GetMessageByIdAsync(int id);
        Task<String> AddMessageAsync(MessagesDTO messagesDTO);
        Task<String> UpdateMessagesAsync(MessagesDTO messagesDTO);
        Task DeleteMessageAsync(int id);
    }
}
