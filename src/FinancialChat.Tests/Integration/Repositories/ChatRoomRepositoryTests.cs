using FinancialChat.Core.Entities;
using FinancialChat.Infrastructure.Persistence.Context;
using FinancialChat.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinancialChat.Tests.Integration.Repositories;

public class ChatRoomRepositoryTests : IDisposable
{
    private readonly FinancialChatDbContext _context;
    private readonly ChatRoomRepository _repository;

    public ChatRoomRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<FinancialChatDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new FinancialChatDbContext(options);
        _repository = new ChatRoomRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region GetByNameAsync Tests

    [Fact]
    public async Task GetByNameAsync_WithExistingName_ReturnsRoom()
    {
        // Arrange
        var chatRoom = new ChatRoom
        {
            Id = Guid.NewGuid(),
            Name = "General",
            Description = "General chat",
            CreatedAt = DateTime.UtcNow
        };
        _context.ChatRooms.Add(chatRoom);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAsync("General");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("General");
        result.Description.Should().Be("General chat");
    }

    [Fact]
    public async Task GetByNameAsync_WithNonExistingName_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByNameAsync("NonExistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByNameAsync_IsCaseSensitive()
    {
        // Arrange
        var chatRoom = new ChatRoom
        {
            Id = Guid.NewGuid(),
            Name = "General",
            CreatedAt = DateTime.UtcNow
        };
        _context.ChatRooms.Add(chatRoom);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAsync("general");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByNameAsync_WithMultipleRooms_ReturnsCorrectOne()
    {
        // Arrange
        var rooms = new List<ChatRoom>
        {
            new() { Id = Guid.NewGuid(), Name = "General", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Tech", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Random", CreatedAt = DateTime.UtcNow }
        };
        _context.ChatRooms.AddRange(rooms);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAsync("Tech");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Tech");
    }

    #endregion

    #region GetAllWithMessageCountAsync Tests

    [Fact]
    public async Task GetAllWithMessageCountAsync_WithNoRooms_ReturnsEmptyList()
    {
        // Act
        var result = await _repository.GetAllWithMessageCountAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllWithMessageCountAsync_WithRooms_ReturnsAll()
    {
        // Arrange
        var rooms = new List<ChatRoom>
        {
            new() { Id = Guid.NewGuid(), Name = "Room1", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Room2", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Room3", CreatedAt = DateTime.UtcNow }
        };
        _context.ChatRooms.AddRange(rooms);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllWithMessageCountAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllWithMessageCountAsync_IncludesMessages()
    {
        // Arrange
        var room = new ChatRoom
        {
            Id = Guid.NewGuid(),
            Name = "General",
            CreatedAt = DateTime.UtcNow,
            Messages = new List<Message>
            {
                new() { Id = Guid.NewGuid(), Content = "Hello", UserName = "user1", Timestamp = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Content = "World", UserName = "user2", Timestamp = DateTime.UtcNow }
            }
        };
        _context.ChatRooms.Add(room);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllWithMessageCountAsync();

        // Assert
        var returnedRoom = result.First();
        returnedRoom.Messages.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllWithMessageCountAsync_WithMixedMessageCounts_ReturnsCorrectCounts()
    {
        // Arrange
        var room1 = new ChatRoom
        {
            Id = Guid.NewGuid(),
            Name = "Empty",
            CreatedAt = DateTime.UtcNow,
            Messages = new List<Message>()
        };

        var room2 = new ChatRoom
        {
            Id = Guid.NewGuid(),
            Name = "Active",
            CreatedAt = DateTime.UtcNow,
            Messages = new List<Message>
            {
                new() { Id = Guid.NewGuid(), Content = "Msg1", UserName = "user", Timestamp = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Content = "Msg2", UserName = "user", Timestamp = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), Content = "Msg3", UserName = "user", Timestamp = DateTime.UtcNow }
            }
        };

        _context.ChatRooms.AddRange(room1, room2);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllWithMessageCountAsync()).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Single(r => r.Name == "Empty").Messages.Should().BeEmpty();
        result.Single(r => r.Name == "Active").Messages.Should().HaveCount(3);
    }

    #endregion

    #region Base Repository Methods Tests

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsRoom()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var chatRoom = new ChatRoom
        {
            Id = roomId,
            Name = "Test Room",
            CreatedAt = DateTime.UtcNow
        };
        _context.ChatRooms.Add(chatRoom);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(roomId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(roomId);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_AddsRoomToDatabase()
    {
        // Arrange
        var chatRoom = new ChatRoom
        {
            Id = Guid.NewGuid(),
            Name = "New Room",
            Description = "A new chat room",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _repository.CreateAsync(chatRoom);
        await _context.SaveChangesAsync();

        // Assert
        var saved = await _context.ChatRooms.FindAsync(chatRoom.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("New Room");
    }

    [Fact]
    public async Task DeleteAsync_MarksRoomAsDeleted()
    {
        // Arrange
        var chatRoom = new ChatRoom
        {
            Id = Guid.NewGuid(),
            Name = "Room to Delete",
            CreatedAt = DateTime.UtcNow
        };
        _context.ChatRooms.Add(chatRoom);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(chatRoom);
        await _context.SaveChangesAsync();

        // Assert - soft delete should mark IsDeleted
        var deleted = await _context.ChatRooms
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == chatRoom.Id);

        deleted.Should().NotBeNull();
        deleted!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllRooms()
    {
        // Arrange
        var rooms = new List<ChatRoom>
        {
            new() { Id = Guid.NewGuid(), Name = "Room1", CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Room2", CreatedAt = DateTime.UtcNow }
        };
        _context.ChatRooms.AddRange(rooms);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    #endregion
}
