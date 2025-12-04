using System.Reflection;
using FinancialChat.Core.Entities;
using FinancialChat.Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinancialChat.Infrastructure.Persistence.Context;

public class FinancialChatDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    private readonly SoftDeleteInterceptor _softDeleteInterceptor;

    public FinancialChatDbContext(DbContextOptions<FinancialChatDbContext> options)
        : base(options)
    {
        _softDeleteInterceptor = new SoftDeleteInterceptor();
    }

    public DbSet<ChatRoom> ChatRooms => Set<ChatRoom>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_softDeleteInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global delete behavior - prevent cascade deletes
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }
    }
}
