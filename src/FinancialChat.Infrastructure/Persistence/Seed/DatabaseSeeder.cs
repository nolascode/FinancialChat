using FinancialChat.Core.Entities;
using FinancialChat.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinancialChat.Infrastructure.Persistence.Seed;

public class DatabaseSeeder
{
    private readonly FinancialChatDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(FinancialChatDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
            _logger.LogInformation("Database migrated successfully");

            await SeedChatRoomsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding database");
            throw;
        }
    }

    private async Task SeedChatRoomsAsync()
    {
        if (!await _context.ChatRooms.AnyAsync())
        {
            _logger.LogInformation("Seeding default chat rooms");

            var defaultRooms = new List<ChatRoom>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "General",
                    Description = "General discussion chat room",
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Stocks",
                    Description = "Stock market discussion and quotes",
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Technology",
                    Description = "Technology and innovation discussions",
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _context.ChatRooms.AddRangeAsync(defaultRooms);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created {Count} default chat rooms", defaultRooms.Count);
        }
        else
        {
            _logger.LogInformation("Chat rooms already exist, skipping seed");
        }
    }
}
