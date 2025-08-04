using Chat.DTOS;

namespace Chat.Service.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUSersAsync();
        Task<UserDTO> GetUserByIdAsync(int id);
        Task<UserDTO> AddUserAsync(UserDTO userDTO);
        Task UpdateUserAsync(UserDTO userDTO, int id);
        Task DeleteUserAsync(int id);
        Task<bool> CheckDuplicateUserAsync(string email);
        Task<bool> CheckDuplicateUserAsync(string email, int id);
    }
}
