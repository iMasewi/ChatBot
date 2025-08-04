using Chat.Models;

namespace Chat.Repository.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<bool> CheckDuplicateUserAsync(String email);
        Task<bool> CheckDuplicateUserAsync(String email, int id);
    }
}
