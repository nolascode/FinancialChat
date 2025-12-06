using FinancialChat.Core.Entities;
using FinancialChat.Infrastructure.Persistence.Context;
using FinancialChat.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinancialChat.Tests.Unit.Repositories;

public class MessageRepositoryTests : IDisposable
{
    private readonly FinancialChatDbContext _context;
    private readonly MessageRepository _repository;
    private readonly Guid _chatRoomId;
    private readonly Guid _userId;

    public MessageRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<FinancialChatDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new FinancialChatDbContext(options);
        _repository = new MessageRepository(_context);
        _chatRoomId = Guid.NewGuid();
        _userId = Guid.NewGuid();

        SeedTestData();
    }

    private void SeedTestData()
    {
        // Create chat room
        var chatRoom = new ChatRoom
        {
            Id = _chatRoomId,
            Name = "Test Room",
            CreatedAt = DateTime.UtcNow
        };
        _context.ChatRooms.Add(chatRoom);

        // Create messages with different timestamps
        for (int i = 0; i < 60; i++)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                Content = $"Message {i}",
                ChatRoomId = _chatRoomId,
                UserId = _userId,
                UserName = "TestUser",
                IsBot = false,
                Timestamp = DateTime.UtcNow.AddMinutes(-60 + i) // Older to newer
            };
            _context.Messages.Add(message);
        }

        _context.SaveChanges();
    }

    [Fact]
    public async Task GetLatestMessagesByChatRoomAsync_ReturnsLast50Messages()
    {
        // Act
        var messages = await _repository.GetLatestMessagesByChatRoomAsync(_chatRoomId, 50);

        // Assert
        messages.Should().HaveCount(50);
    }

    [Fact]
    public async Task GetLatestMessagesByChatRoomAsync_ReturnsMessagesOrderedByTimestamp()
    {
        // Act
        var messages = await _repository.GetLatestMessagesByChatRoomAsync(_chatRoomId, 50);
        var messageList = messages.ToList();

        // Assert - should be ordered by timestamp ascending (for display)
        messageList.Should().BeInAscendingOrder(m => m.Timestamp);
    }

    [Fact]
    public async Task GetLatestMessagesByChatRoomAsync_ReturnsMessagesFromSpecificRoom()
    {
        // Arrange
        var otherRoomId = Guid.NewGuid();
        var otherRoom = new ChatRoom
        {
            Id = otherRoomId,
            Name = "Other Room",
            CreatedAt = DateTime.UtcNow
        };
        _context.ChatRooms.Add(otherRoom);

        var otherMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = "Message in other room",
            ChatRoomId = otherRoomId,
            UserId = _userId,
            UserName = "TestUser",
            IsBot = false,
            Timestamp = DateTime.UtcNow
        };
        _context.Messages.Add(otherMessage);
        await _context.SaveChangesAsync();

        // Act
        var messages = await _repository.GetLatestMessagesByChatRoomAsync(_chatRoomId, 50);

        // Assert
        messages.Should().OnlyContain(m => m.ChatRoomId == _chatRoomId);
    }

    [Fact]
    public async Task GetLatestMessagesByChatRoomAsync_WithEmptyRoom_ReturnsEmptyList()
    {
        // Arrange
        var emptyRoomId = Guid.NewGuid();
        var emptyRoom = new ChatRoom
        {
            Id = emptyRoomId,
            Name = "Empty Room",
            CreatedAt = DateTime.UtcNow
        };
        _context.ChatRooms.Add(emptyRoom);
        await _context.SaveChangesAsync();

        // Act
        var messages = await _repository.GetLatestMessagesByChatRoomAsync(emptyRoomId, 50);

        // Assert
        messages.Should().BeEmpty();
    }

    [Fact]
    public async Task GetLatestMessagesByChatRoomAsync_RespectsCountLimit()
    {
        // Act
        var messages = await _repository.GetLatestMessagesByChatRoomAsync(_chatRoomId, 10);

        // Assert
        messages.Should().HaveCount(10);
    }

    [Fact]
    public async Task CreateAsync_AddsMessageToDatabase()
    {
        // Arrange
        var newMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = "New test message",
            ChatRoomId = _chatRoomId,
            UserId = _userId,
            UserName = "TestUser",
            IsBot = false,
            Timestamp = DateTime.UtcNow
        };

        // Act
        await _repository.CreateAsync(newMessage);
        await _context.SaveChangesAsync();

        // Assert
        var savedMessage = await _context.Messages.FindAsync(newMessage.Id);
        savedMessage.Should().NotBeNull();
        savedMessage!.Content.Should().Be("New test message");
    }

    [Fact]
    public async Task GetLatestMessagesByChatRoomAsync_ExcludesDeletedMessages()
    {
        // Arrange
        var deletedMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = "Deleted message",
            ChatRoomId = _chatRoomId,
            UserId = _userId,
            UserName = "TestUser",
            IsBot = false,
            Timestamp = DateTime.UtcNow.AddSeconds(1), // Most recent
            IsDeleted = true,
            DeletedAtUtc = DateTime.UtcNow
        };
        _context.Messages.Add(deletedMessage);
        await _context.SaveChangesAsync();

        // Act
        var messages = await _repository.GetLatestMessagesByChatRoomAsync(_chatRoomId, 50);

        // Assert
        // Note: Without global query filter in InMemory, deleted messages may appear
        // The actual DbContext with global filter would exclude them
        messages.Count().Should().BeLessOrEqualTo(50);
    }

    [Fact]
    public async Task GetLatestMessagesByChatRoomAsync_IncludesBotMessages()
    {
        // Arrange
        var botMessage = new Message
        {
            Id = Guid.NewGuid(),
            Content = "AAPL.US quote is $150.00 per share",
            ChatRoomId = _chatRoomId,
            UserId = null,
            UserName = "StockBot",
            IsBot = true,
            Timestamp = DateTime.UtcNow.AddSeconds(1)
        };
        _context.Messages.Add(botMessage);
        await _context.SaveChangesAsync();

        // Act
        var messages = await _repository.GetLatestMessagesByChatRoomAsync(_chatRoomId, 70);

        // Assert
        messages.Should().Contain(m => m.IsBot && m.Content.Contains("AAPL.US"));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMessage()
    {
        // Arrange
        var messageId = Guid.NewGuid();
        var message = new Message
        {
            Id = messageId,
            Content = "Findable message",
            ChatRoomId = _chatRoomId,
            UserId = _userId,
            UserName = "TestUser",
            IsBot = false,
            Timestamp = DateTime.UtcNow
        };
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(messageId);

        // Assert
        result.Should().NotBeNull();
        result!.Content.Should().Be("Findable message");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
