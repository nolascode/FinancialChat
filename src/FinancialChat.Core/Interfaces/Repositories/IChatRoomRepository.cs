using FinancialChat.Core.Entities;

namespace FinancialChat.Core.Interfaces.Repositories;

public interface IChatRoomRepository : IRepositoryBase<ChatRoom>
{
    Task<ChatRoom?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<ChatRoom>> GetAllWithMessageCountAsync(CancellationToken cancellationToken = default);
}
