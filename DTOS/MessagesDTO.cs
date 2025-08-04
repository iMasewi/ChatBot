namespace Chat.DTOS
{
    public class MessagesDTO
    {
        public int Id { get; set; }
        public string? User { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int ConversationId { get; set; }
    }
}
