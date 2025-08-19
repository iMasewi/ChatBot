using Chat.DTOS;
using Chat.Models;
using Chat.Service.Interfaces;
using LoginUpLevel.DTOs;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO loginDTO)
        {
            if (loginDTO == null || string.IsNullOrEmpty(loginDTO.Email) || string.IsNullOrEmpty(loginDTO.Password))
            {
                return BadRequest("Invalid login data.");
            }
            try
            {
                var token = await _authService.LoginAsync(loginDTO);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CheckStatus()
        {
            try
            {
                var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(id, out var userId))
                {
                    return BadRequest("Invalid customer ID in user claims.");
                }

                var name = User.FindFirst(ClaimTypes.Name).Value;
                var email = User.FindFirst(ClaimTypes.Email).Value;
                var role = User.FindFirst(ClaimTypes.Role).Value;

                var createdAtString = User.FindFirst("createdAt")?.Value;
                DateTime createdAt = DateTime.MinValue;

                if (!string.IsNullOrEmpty(createdAtString))
                {
                    DateTime.TryParse(createdAtString, out createdAt);
                }

                var user = new UserDTO
                {
                    Id = userId,
                    Name = name,
                    Email = email,
                    Role = role,
                    CreatedAt = createdAt
                };
                return Ok(user);
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet("loginGoogle")]
        public IActionResult LoginGoogle([FromQuery] string returnUrl, LinkGenerator linkGenerator, SignInManager<User> signManager)
        {
            var callbackUrl = linkGenerator.GetPathByName(HttpContext, "GoogleLoginCallback")
                              + $"?returnUrl={returnUrl}";

            var properties = signManager.ConfigureExternalAuthenticationProperties("Google", callbackUrl);

            return Challenge(properties, "Google");
        }


        [HttpGet("loginGoogle/callback", Name = "GoogleLoginCallback")]
        public async Task<IActionResult> GoogleLoginCallback([FromQuery] string returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            var token = await _authService.LoginWithGoogleAsync(result.Principal);

            return Redirect($"{returnUrl}?token={token}");
        }
    }
}
