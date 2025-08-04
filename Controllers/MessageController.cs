using Chat.DTOS;
using Chat.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Chat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessagesService _messagesService;
        private readonly HttpClient _httpClient;
        public MessageController(IMessagesService messagesService, IHttpClientFactory httpClient)
        {
            _messagesService = messagesService;
            _httpClient = httpClient.CreateClient();
        }

        [HttpPost()]
        [Authorize]
        public async Task<IActionResult> AddMessageAsync([FromBody] MessagesDTO messagesDTO)
        {
            if (messagesDTO == null)
            {
                return BadRequest("Message data cannot be null.");
            }
            try
            {
                messagesDTO.User = "User";
                var result = await _messagesService.AddMessageAsync(messagesDTO);

                var response = await _httpClient.GetAsync($"http://127.0.0.1:8000/search?query={messagesDTO.Content}");
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Error from external service: {responseContent}");
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
                var newMessageResult = await _messagesService.AddMessageAsync(newMessageDTO);

                return Ok(new {content = content}); 
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessageAsync(int id)
        {
            try
            {
                await _messagesService.DeleteMessageAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("conversation/{conversationId}")]
        public async Task<IActionResult> GetMessagesByConversationIdAsync(int conversationId)
        {
            try
            {
                var messages = await _messagesService.GetMessagesByConversationIdAsync(conversationId);
                if (messages == null)
                {
                    return NotFound("No messages found for this conversation.");
                }
                return Ok(messages);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
