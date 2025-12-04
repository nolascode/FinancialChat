namespace FinancialChat.Core.Entities;

public interface ISoftDeleteEntity
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAtUtc { get; set; }
}
