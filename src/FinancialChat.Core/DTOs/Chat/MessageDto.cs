namespace FinancialChat.Core.DTOs.Chat;

public record MessageDto
{
    public Guid Id { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string UserName { get; init; } = string.Empty;
    public bool IsBot { get; init; }
    public Guid ChatRoomId { get; init; }
}
