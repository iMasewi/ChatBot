using Chat.DTOS;

namespace Chat.Service.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(UserDTO userDto);
    }
}
