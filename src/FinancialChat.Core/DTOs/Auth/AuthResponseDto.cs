namespace FinancialChat.Core.DTOs.Auth;

public record AuthResponseDto
{
    public bool Success { get; init; }
    public string? Token { get; init; }
    public DateTime? Expiration { get; init; }
    public string? UserId { get; init; }
    public string? UserName { get; init; }
    public string? Email { get; init; }
    public string? Message { get; init; }
    public IEnumerable<string>? Errors { get; init; }
}
