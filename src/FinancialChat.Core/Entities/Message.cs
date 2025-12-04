namespace FinancialChat.Core.Entities;

public class Message : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string UserName { get; set; } = string.Empty;
    public bool IsBot { get; set; }

    // Foreign keys
    public Guid? UserId { get; set; }
    public Guid ChatRoomId { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual ChatRoom ChatRoom { get; set; } = null!;
}
