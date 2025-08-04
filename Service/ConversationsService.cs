using AutoMapper;
using Chat.DTOS;
using Chat.Models;
using Chat.Repository.Interfaces;
using Chat.Service.Interfaces;

namespace Chat.Service
{
    public class ConversationsService : IConversationsService
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IMapper _mapper;
        public ConversationsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<int> AddConversationAsync(ConversationsDTO conversationsDTO)
        {
            try
            {
                var conversation = _mapper.Map<Conversations>(conversationsDTO);
                if (conversation == null)
                {
                    throw new ArgumentNullException(nameof(conversationsDTO), "Conversation data cannot be null.");
                }
                await _unitOfWork.ConversationsRepository.AddAsync(conversation);
                await _unitOfWork.SaveChangesAsync();
                return conversation.Id;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while adding the conversation.", ex);
            }
        }

        public async Task DeleteConversationAsync(int id)
        {
            try
            {
                var conversation = await _unitOfWork.ConversationsRepository.GetById(id);
                if (conversation == null)
                {
                    throw new KeyNotFoundException($"Conversation with ID {id} not found.");
                }
                await _unitOfWork.ConversationsRepository.DeleteAsync(conversation);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while deleting the conversation.", ex);
            }
        }

        public async Task<IEnumerable<ConversationsDTO>> GetAllConversationsAsync()
        {
            try
            {
                var conversations = await _unitOfWork.ConversationsRepository.GetAllAsync();
                if (conversations == null)
                {
                    throw new KeyNotFoundException("No conversations found.");
                }
                return _mapper.Map<IEnumerable<ConversationsDTO>>(conversations);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while retrieving all conversations.", ex);
            }
        }

        public async Task<ConversationsDTO> GetConversationByIdAsync(int id)
        {
            try
            {
                var conversation = await _unitOfWork.ConversationsRepository.GetById(id);
                if (conversation == null)
                {
                    throw new KeyNotFoundException($"Conversation with ID {id} not found.");
                }
                return _mapper.Map<ConversationsDTO>(conversation);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while retrieving the conversation by ID.", ex);
            }
        }

        public async Task<IEnumerable<ConversationsDTO>> GetConversationsByUserIdAsync(int userId)
        {
            try
            {
                var conversations = await _unitOfWork.ConversationsRepository.GetConversationsByUserIdAsync(userId);
                if (conversations == null || !conversations.Any())
                {
                    throw new KeyNotFoundException($"No conversations found for user ID {userId}.");
                }
                return _mapper.Map<IEnumerable<ConversationsDTO>>(conversations);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("An error occurred while retrieving conversations by user ID.", ex);
            }
        }
        public async Task<int> CountConversationsAsync(int userId)
        {
            return await _unitOfWork.ConversationsRepository.CountConversationAsync(userId);
        }
    }
}
