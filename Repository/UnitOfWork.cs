using Chat.Repository.Interfaces;

namespace Chat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Data.ApplicationDbContext _context;
        public UnitOfWork(Data.ApplicationDbContext context)
        {
            _context = context;
            UserRepository = new UserRepository(_context);
            MessagesRepository = new MessagesRepository(_context);
            ConversationsRepository = new ConversationsRepository(_context);
        }

        public IMessagesRepository MessagesRepository { get; private set; }

        public IUserRepository UserRepository { get; private set; }

        public IConversationsRepository ConversationsRepository { get; private set; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
