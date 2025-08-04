using Microsoft.AspNetCore.Identity;

namespace Chat.Models
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Conversations> Conversations { get; } = new List<Conversations>();
    }
}