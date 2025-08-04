using AutoMapper;
using Chat.DTOS;
using Chat.Repository.Interfaces;
using Chat.Service.Interfaces;

namespace BotChat.Service
{
    public class MessagesService : IMessagesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MessagesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<MessagesDTO> AddMessageAsync(MessagesDTO messagesDTO)
        {
            try
            {
                var message = _mapper.Map<Chat.Models.Messages>(messagesDTO);
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(messagesDTO), "Message data cannot be null.");
                }
                await _unitOfWork.MessagesRepository.AddAsync(message);
                await _unitOfWork.SaveChangesAsync();
                return messagesDTO;

            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while adding the message.", ex);
            }
        }

        public async Task DeleteMessageAsync(int id)
        {
            try
            {
                var message = await _unitOfWork.MessagesRepository.GetById(id);
                if (message == null)
                {
                    throw new KeyNotFoundException($"Message with ID {id} not found.");
                }
                await _unitOfWork.MessagesRepository.DeleteAsync(message);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while deleting the message.", ex);
            }
        }
        public async Task<IEnumerable<MessagesDTO>> GetMessagesByConversationIdAsync(int conversationId)
        {
            try
            {
                var messages = await _unitOfWork.MessagesRepository.GetMessagesByConversationIdAsync(conversationId);
                if (messages == null)
                {
                    throw new KeyNotFoundException("No messages found.");
                }
                return _mapper.Map<IEnumerable<MessagesDTO>>(messages);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while retrieving all messages.", ex);
            }
        }
        public async Task<MessagesDTO> GetMessageByIdAsync(int id)
        {
            try
            {
                var message = await _unitOfWork.MessagesRepository.GetById(id);
                if (message == null)
                {
                    throw new KeyNotFoundException($"Message with ID {id} not found.");
                }
                return _mapper.Map<MessagesDTO>(message);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while retrieving the message by ID.", ex);
            }
        }
    }
}
