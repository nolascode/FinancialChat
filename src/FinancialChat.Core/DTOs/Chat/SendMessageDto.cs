using System.ComponentModel.DataAnnotations;

namespace FinancialChat.Core.DTOs.Chat;

public record SendMessageDto
{
    [Required(ErrorMessage = "Message content is required")]
    [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
    public string Content { get; init; } = string.Empty;

    [Required(ErrorMessage = "Chat room is required")]
    public Guid ChatRoomId { get; init; }
}
