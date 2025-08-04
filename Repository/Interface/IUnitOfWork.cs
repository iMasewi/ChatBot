namespace Chat.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IMessagesRepository MessagesRepository { get; }
        IUserRepository UserRepository { get; }
        IConversationsRepository ConversationsRepository { get; }
        Task SaveChangesAsync();
        void Dispose();
    }
}
