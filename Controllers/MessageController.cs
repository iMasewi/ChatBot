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
        
        public MessageController(IMessagesService messagesService)
        {
            _messagesService = messagesService;
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
                string content = await _messagesService.AddMessageAsync(messagesDTO);
                if (string.IsNullOrEmpty(content))
                {
                    return BadRequest("Failed to add message.");
                }
                return Ok(new { Content = content });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateMessageAsync([FromBody] MessagesDTO messagesDTO)
        {
            if (messagesDTO == null)
            {
                return BadRequest("Message data cannot be null.");
            }
            try
            {
                string content = await _messagesService.UpdateMessagesAsync(messagesDTO);
                if (string.IsNullOrEmpty(content))
                {
                    return BadRequest("Failed to update message.");
                }
                return Ok(content);
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
