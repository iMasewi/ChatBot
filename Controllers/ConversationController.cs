using Chat.DTOS;
using Chat.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationsService _conversationsService;
        public ConversationController(IConversationsService conversationsService)
        {
            _conversationsService = conversationsService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddConversationAsync([FromBody] ConversationsDTO conversationsDTO)
        {
            if (conversationsDTO == null)
            {
                return BadRequest("Conversation data cannot be null.");
            }
            try
            {
                var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(Id, out var userId))
                {
                    return BadRequest("Invalid customer ID in user claims.");
                }

                conversationsDTO.UserId = userId;
                var result = await _conversationsService.AddConversationAsync(conversationsDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllConversationsAsync()
        {
            try
            {
                var conversations = await _conversationsService.GetAllConversationsAsync();
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConversationByIdAsync(int id)
        {
            try
            {
                var conversation = await _conversationsService.GetConversationByIdAsync(id);
                if (conversation == null)
                {
                    return NotFound($"Conversation with ID {id} not found.");
                }
                return Ok(conversation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetConversationsByUserIdAsync()
        {
            try
            {
                var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(Id, out var userId))
                {
                    return BadRequest("Invalid customer ID in user claims.");
                }

                var conversations = await _conversationsService.GetConversationsByUserIdAsync(userId);
                if (conversations == null || !conversations.Any())
                {
                    return NotFound($"No conversations found for user ID {userId}.");
                }
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("count")]
        [Authorize]
        public async Task<IActionResult> GetConversationCountAsync()
        {
            try
            {
                var Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(Id, out var userId))
                {
                    return BadRequest("Invalid customer ID in user claims.");
                }
                var count = await _conversationsService.CountConversationsAsync(userId);
                return Ok(new { Count = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteConversationAsync(int id)
        {
            try
            {
                await _conversationsService.DeleteConversationAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Conversation with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
