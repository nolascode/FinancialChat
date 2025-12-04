using System.Text.Json;

namespace FinancialChat.Core.DTOs.Common;

public record ErrorResponse
{
    public int StatusCode { get; init; }
    public string? Message { get; init; }

    public override string ToString() => JsonSerializer.Serialize(this);
}
