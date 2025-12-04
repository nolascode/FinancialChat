using FinancialChat.Core.Entities;

namespace FinancialChat.Core.Interfaces.Repositories;

public interface IMessageRepository : IRepositoryBase<Message>
{
    Task<IEnumerable<Message>> GetLatestMessagesByChatRoomAsync(Guid chatRoomId, int count = 50, CancellationToken cancellationToken = default);

    Task<(IEnumerable<Message> Messages, int TotalCount)> GetMessagesByChatRoomPagedAsync(
        Guid chatRoomId,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
}
