using Chat.Data;
using Chat.Models;
using Chat.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chat.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
        public Task<bool> CheckDuplicateUserAsync(string email)
        {
            return _context.Users.AnyAsync(u => u.Email == email);
        }

        public Task<bool> CheckDuplicateUserAsync(string email, int id)
        {
            return _context.Users
                .AnyAsync(u => u.Email == email && u.Id != id);
        }
    }
}
