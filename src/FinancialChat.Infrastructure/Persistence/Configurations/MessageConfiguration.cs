using FinancialChat.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinancialChat.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasQueryFilter(m => !m.IsDeleted);

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.UserName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Timestamp)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(m => m.Timestamp);
        builder.HasIndex(m => m.ChatRoomId);
    }
}
