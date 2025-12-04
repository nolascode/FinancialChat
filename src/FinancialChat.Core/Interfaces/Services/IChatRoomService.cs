using FinancialChat.Core.DTOs.Chat;
using FinancialChat.Core.DTOs.Common;

namespace FinancialChat.Core.Interfaces.Services;

/// <summary>
/// Defines chat room service operations for managing chat rooms and their messages.
/// </summary>
public interface IChatRoomService
{
    /// <summary>
    /// Gets all chat rooms with their message counts.
    /// </summary>
    Task<IEnumerable<ChatRoomDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a chat room by its unique identifier.
    /// </summary>
    /// <exception cref="Core.Exceptions.NotFoundException">Thrown when chat room is not found.</exception>
    Task<ChatRoomDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated messages for a specific chat room.
    /// </summary>
    /// <exception cref="Core.Exceptions.NotFoundException">Thrown when chat room is not found.</exception>
    Task<PagedResult<MessageDto>> GetMessagesAsync(Guid chatRoomId, int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new chat room.
    /// </summary>
    /// <exception cref="Core.Exceptions.ConflictException">Thrown when a room with the same name exists.</exception>
    Task<ChatRoomDto> CreateAsync(CreateChatRoomDto createDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a chat room by its identifier.
    /// </summary>
    /// <exception cref="Core.Exceptions.NotFoundException">Thrown when chat room is not found.</exception>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
