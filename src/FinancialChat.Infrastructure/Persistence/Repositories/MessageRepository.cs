using FinancialChat.Core.Entities;
using FinancialChat.Core.Interfaces.Repositories;
using FinancialChat.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FinancialChat.Infrastructure.Persistence.Repositories;

public class MessageRepository : RepositoryBase<Message>, IMessageRepository
{
    public MessageRepository(FinancialChatDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Message>> GetLatestMessagesByChatRoomAsync(
        Guid chatRoomId,
        int count = 50,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(m => m.ChatRoomId == chatRoomId)
            .OrderByDescending(m => m.Timestamp)
            .Take(count)
            .OrderBy(m => m.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Message> Messages, int TotalCount)> GetMessagesByChatRoomPagedAsync(
        Guid chatRoomId,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(m => m.ChatRoomId == chatRoomId);

        var totalCount = await query.CountAsync(cancellationToken);

        // Get messages ordered by timestamp descending (newest first for pagination)
        // Skip based on page, then reverse for chronological display
        var messages = await query
            .OrderByDescending(m => m.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .OrderBy(m => m.Timestamp) // Reverse to chronological order
            .ToListAsync(cancellationToken);

        return (messages, totalCount);
    }
}
