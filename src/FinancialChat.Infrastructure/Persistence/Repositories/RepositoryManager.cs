using FinancialChat.Core.Interfaces.Repositories;
using FinancialChat.Infrastructure.Persistence.Context;

namespace FinancialChat.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implements IRepositoryManager to manage database transactions.
/// </summary>
public sealed class RepositoryManager : IRepositoryManager
{
    private readonly FinancialChatDbContext _context;
    private readonly Lazy<IMessageRepository> _messageRepository;
    private readonly Lazy<IChatRoomRepository> _chatRoomRepository;

    public RepositoryManager(FinancialChatDbContext context)
    {
        _context = context;
        _messageRepository = new Lazy<IMessageRepository>(() => new MessageRepository(context));
        _chatRoomRepository = new Lazy<IChatRoomRepository>(() => new ChatRoomRepository(context));
    }

    public IMessageRepository MessageRepository => _messageRepository.Value;
    public IChatRoomRepository ChatRoomRepository => _chatRoomRepository.Value;

    public async Task SaveAsync() => await _context.SaveChangesAsync();
}
