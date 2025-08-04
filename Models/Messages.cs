namespace Chat.Models
{
    public class Messages
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ConversationId { get; set; }
        public Conversations Conversation { get; set; }
    }
}
