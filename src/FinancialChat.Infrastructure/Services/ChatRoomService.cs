using FinancialChat.Core.DTOs.Chat;
using FinancialChat.Core.DTOs.Common;
using FinancialChat.Core.Entities;
using FinancialChat.Core.Exceptions;
using FinancialChat.Core.Interfaces.Repositories;
using FinancialChat.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace FinancialChat.Infrastructure.Services;

/// <summary>
/// Implements chat room service operations for managing chat rooms and messages.
/// </summary>
public class ChatRoomService : IChatRoomService
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILogger<ChatRoomService> _logger;

    public ChatRoomService(
        IRepositoryManager repositoryManager,
        ILogger<ChatRoomService> logger)
    {
        _repositoryManager = repositoryManager;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ChatRoomDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var chatRooms = await _repositoryManager.ChatRoomRepository.GetAllWithMessageCountAsync(cancellationToken);

        return chatRooms.Select(c => new ChatRoomDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            CreatedAt = c.CreatedAt,
            MessageCount = c.Messages.Count
        });
    }

    /// <inheritdoc />
    public async Task<ChatRoomDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var chatRoom = await _repositoryManager.ChatRoomRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Chat room with id '{id}' not found");

        return new ChatRoomDto
        {
            Id = chatRoom.Id,
            Name = chatRoom.Name,
            Description = chatRoom.Description,
            CreatedAt = chatRoom.CreatedAt
        };
    }

    /// <inheritdoc />
    public async Task<PagedResult<MessageDto>> GetMessagesAsync(
        Guid chatRoomId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        _ = await _repositoryManager.ChatRoomRepository.GetByIdAsync(chatRoomId, cancellationToken)
            ?? throw new NotFoundException($"Chat room with id '{chatRoomId}' not found");

        // Clamp values
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (messages, totalCount) = await _repositoryManager.MessageRepository
            .GetMessagesByChatRoomPagedAsync(chatRoomId, page, pageSize, cancellationToken);

        var dtos = messages.Select(m => new MessageDto
        {
            Id = m.Id,
            Content = m.Content,
            Timestamp = m.Timestamp,
            UserName = m.UserName,
            IsBot = m.IsBot,
            ChatRoomId = m.ChatRoomId
        });

        return new PagedResult<MessageDto>
        {
            Items = dtos,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    /// <inheritdoc />
    public async Task<ChatRoomDto> CreateAsync(CreateChatRoomDto createDto, CancellationToken cancellationToken = default)
    {
        var existing = await _repositoryManager.ChatRoomRepository.GetByNameAsync(createDto.Name, cancellationToken);
        if (existing != null)
        {
            throw new ConflictException($"Chat room with name '{createDto.Name}' already exists");
        }

        var chatRoom = new ChatRoom
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Description = createDto.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _repositoryManager.ChatRoomRepository.CreateAsync(chatRoom, cancellationToken);
        await _repositoryManager.SaveAsync();

        _logger.LogInformation("Chat room {Name} created", chatRoom.Name);

        return new ChatRoomDto
        {
            Id = chatRoom.Id,
            Name = chatRoom.Name,
            Description = chatRoom.Description,
            CreatedAt = chatRoom.CreatedAt,
            MessageCount = 0
        };
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var chatRoom = await _repositoryManager.ChatRoomRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Chat room with id '{id}' not found");

        await _repositoryManager.ChatRoomRepository.DeleteAsync(chatRoom, cancellationToken);
        await _repositoryManager.SaveAsync();

        _logger.LogInformation("Chat room {Name} deleted", chatRoom.Name);
    }
}
