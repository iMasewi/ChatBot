namespace Chat.DTOS
{
    public class ConversationsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public ICollection<MessagesDTO> Messages { get; } = new List<MessagesDTO>();
    }
}
