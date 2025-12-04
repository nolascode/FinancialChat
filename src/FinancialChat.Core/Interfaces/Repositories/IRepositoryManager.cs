namespace FinancialChat.Core.Interfaces.Repositories;

/// <summary>
/// Defines a contract for managing repository transactions.
/// </summary>
public interface IRepositoryManager
{
    IMessageRepository MessageRepository { get; }
    IChatRoomRepository ChatRoomRepository { get; }

    /// <summary>
    /// Asynchronously saves changes to the database.
    /// </summary>
    Task SaveAsync();
}
