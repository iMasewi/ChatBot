namespace Chat.Models
{
    public class Conversations
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Messages> Messages { get; } = new List<Messages>();
    }
}
