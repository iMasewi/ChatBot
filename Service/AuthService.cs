using Chat.DTOS;
using Chat.Models;
using Chat.Repository;
using Chat.Repository.Interfaces;
using Chat.Service.Interfaces;
using LoginUpLevel.DTOs;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Chat.Service
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthService(IJwtService jwtService, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public async Task<string> LoginAsync(LoginDTO loginDTO)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginDTO.Email);
                var result = await _signInManager.PasswordSignInAsync(user, loginDTO.Password, false, false);
                if (user == null || result == null)
                {
                    throw new UnauthorizedAccessException("Invalid information");
                }
                var userDto = await MapToUserDto(user);
                var token = await _jwtService.GenerateTokenAsync(userDto);
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception("Login failed", ex);
            }
        }
        private async Task<UserDTO> MapToUserDto(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
            };
        }
        public async Task<string> LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                throw new Exception("Google orr ClaimsPrinvipal is null");
            }
            var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            if (email == null)
            {
                throw new Exception("Google");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var newUser = new User
                {
                    UserName = email,
                    Email = email,
                    Name = claimsPrincipal.FindFirstValue(ClaimTypes.Name) ?? string.Empty
                };

                var result = await _userManager.CreateAsync(newUser);
                if (!result.Succeeded)
                {
                    throw new Exception("Unable to create user");
                }

                var userRole = await _userManager.AddToRoleAsync(newUser, "User");
                if (!userRole.Succeeded)
                {
                    throw new Exception("Failed to add user to role: ");
                }

                var userDto = await MapToUserDto(newUser);

                var info = new UserLoginInfo("Google",
                claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
                "Google");
                var loginResult = await _userManager.AddLoginAsync(newUser, info);

                if (!loginResult.Succeeded)
                {
                    throw new Exception("Unable to add Google login");
                }

                return await _jwtService.GenerateTokenAsync(userDto);
            }
            else
            {
                var userDto = await MapToUserDto(user);
                return await _jwtService.GenerateTokenAsync(userDto);
            }
        }
    }
}
