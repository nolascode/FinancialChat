using FinancialChat.Core.Entities;
using FinancialChat.Core.Interfaces.Repositories;
using FinancialChat.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FinancialChat.Infrastructure.Persistence.Repositories;

public class ChatRoomRepository : RepositoryBase<ChatRoom>, IChatRoomRepository
{
    public ChatRoomRepository(FinancialChatDbContext context) : base(context)
    {
    }

    public async Task<ChatRoom?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<ChatRoom>> GetAllWithMessageCountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.Messages)
            .ToListAsync(cancellationToken);
    }
}
