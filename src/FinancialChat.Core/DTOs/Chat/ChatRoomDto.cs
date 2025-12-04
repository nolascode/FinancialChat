namespace FinancialChat.Core.DTOs.Chat;

public record ChatRoomDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public int MessageCount { get; init; }
}

public record CreateChatRoomDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}
