using AutoMapper;
using Chat.DTOS;
using Chat.Models;
using Chat.Repository.Interfaces;
using Chat.Service.Interfaces;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BotChat.Service
{
    public class MessagesService : IMessagesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        public MessagesService(IUnitOfWork unitOfWork, IMapper mapper, IHttpClientFactory httpClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpClient = httpClient.CreateClient();
        }
        public async Task<String> AddMessageAsync(MessagesDTO messagesDTO)
        {
            try
            {
                messagesDTO.User = "User";
                var result = await AddAsync(messagesDTO);

                var response = await _httpClient.GetAsync($"http://127.0.0.1:8000/search?query={messagesDTO.Content}");
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error fetching data from external service: {responseContent}");
                }

                // Parse JSON để lấy giá trị "context"
                using var jsonDoc = JsonDocument.Parse(responseContent);
                string content = jsonDoc.RootElement.GetProperty("context").GetString();

                var newMessageDTO = new MessagesDTO
                {
                    User = "AI",
                    Content = content,
                    ConversationId = messagesDTO.ConversationId
                };
                var newMessageResult = await AddAsync(newMessageDTO);
                return content;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
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

        public async Task<String> UpdateMessagesAsync(MessagesDTO messagesDTO)
        {
            try
            {
                var message = await _unitOfWork.MessagesRepository.GetById(messagesDTO.Id);
                messagesDTO.Id = 0;
                if (message == null)
                {
                    throw new KeyNotFoundException($"Message with ID {messagesDTO.Id} not found.");
                }

                IEnumerable<Messages> messages = await _unitOfWork.MessagesRepository.GetMessageAndFollowingAsync(message);
                foreach (var item in messages)
                {
                    await _unitOfWork.MessagesRepository.DeleteAsync(item);
                }
                await _unitOfWork.SaveChangesAsync();

                string content = await AddMessageAsync(messagesDTO);
                return content;
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private async Task<MessagesDTO> AddAsync(MessagesDTO messagesDTO)
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

    }
}
