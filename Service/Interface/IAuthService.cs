using LoginUpLevel.DTOs;
using System.Security.Claims;

namespace Chat.Service.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginDTO loginDTO);
        Task<string> LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal);
    }
}
