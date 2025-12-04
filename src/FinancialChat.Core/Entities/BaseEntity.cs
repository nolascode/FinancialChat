namespace FinancialChat.Core.Entities;

public abstract class BaseEntity : ISoftDeleteEntity
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
}
