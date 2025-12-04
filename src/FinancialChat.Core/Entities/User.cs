using Microsoft.AspNetCore.Identity;

namespace FinancialChat.Core.Entities;

public class User : IdentityUser<Guid>, ISoftDeleteEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }

    // Navigation properties
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
