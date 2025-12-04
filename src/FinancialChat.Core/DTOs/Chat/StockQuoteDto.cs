namespace FinancialChat.Core.DTOs.Chat;

public record StockQuoteDto
{
    public string StockCode { get; init; } = string.Empty;
    public decimal? Price { get; init; }
    public string Message { get; init; } = string.Empty;
    public Guid ChatRoomId { get; init; }
    public Guid CorrelationId { get; init; }
    public bool Success { get; init; }
    public string? Error { get; init; }
}

public record StockRequestDto
{
    public string StockCode { get; init; } = string.Empty;
    public Guid ChatRoomId { get; init; }
    public string RequestedBy { get; init; } = string.Empty;
    public Guid CorrelationId { get; init; }
}
