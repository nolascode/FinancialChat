using FinancialChat.API.Controllers;
using FinancialChat.Core.DTOs.Chat;
using FinancialChat.Core.DTOs.Common;
using FinancialChat.Core.Exceptions;
using FinancialChat.Core.Interfaces.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinancialChat.Tests.Unit.Controllers;

public class ChatRoomControllerTests
{
    private readonly Mock<IServiceManager> _serviceManagerMock;
    private readonly Mock<IChatRoomService> _chatRoomServiceMock;
    private readonly ChatRoomController _controller;

    public ChatRoomControllerTests()
    {
        _serviceManagerMock = new Mock<IServiceManager>();
        _chatRoomServiceMock = new Mock<IChatRoomService>();

        _serviceManagerMock
            .Setup(x => x.ChatRoomService)
            .Returns(_chatRoomServiceMock.Object);

        _controller = new ChatRoomController(_serviceManagerMock.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_WithChatRooms_ReturnsOkWithRooms()
    {
        // Arrange
        var chatRooms = new List<ChatRoomDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "General",
                Description = "General discussion",
                CreatedAt = DateTime.UtcNow,
                MessageCount = 3
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Tech",
                Description = "Tech talk",
                CreatedAt = DateTime.UtcNow,
                MessageCount = 1
            }
        };

        _chatRoomServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatRooms);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<ChatRoomDto>>>().Subject;

        response.Success.Should().BeTrue();
        var rooms = response.Data!.ToList();
        rooms.Should().HaveCount(2);
        rooms[0].Name.Should().Be("General");
        rooms[0].MessageCount.Should().Be(3);
        rooms[1].Name.Should().Be("Tech");
        rooms[1].MessageCount.Should().Be(1);
    }

    [Fact]
    public async Task GetAll_WithNoChatRooms_ReturnsEmptyList()
    {
        // Arrange
        _chatRoomServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ChatRoomDto>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<ChatRoomDto>>>().Subject;

        response.Success.Should().BeTrue();
        response.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_MapsAllFieldsCorrectly()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var createdAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        var chatRooms = new List<ChatRoomDto>
        {
            new()
            {
                Id = roomId,
                Name = "Test Room",
                Description = "Test Description",
                CreatedAt = createdAt,
                MessageCount = 0
            }
        };

        _chatRoomServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatRooms);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<ChatRoomDto>>>().Subject;

        var room = response.Data!.First();
        room.Id.Should().Be(roomId);
        room.Name.Should().Be("Test Room");
        room.Description.Should().Be("Test Description");
        room.CreatedAt.Should().Be(createdAt);
        room.MessageCount.Should().Be(0);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WithExistingRoom_ReturnsOkWithRoom()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var chatRoom = new ChatRoomDto
        {
            Id = roomId,
            Name = "General",
            Description = "General discussion",
            CreatedAt = DateTime.UtcNow
        };

        _chatRoomServiceMock
            .Setup(x => x.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chatRoom);

        // Act
        var result = await _controller.GetById(roomId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<ChatRoomDto>>().Subject;

        response.Success.Should().BeTrue();
        response.Data!.Id.Should().Be(roomId);
        response.Data.Name.Should().Be("General");
    }

    [Fact]
    public async Task GetById_WithNonExistentRoom_ThrowsNotFoundException()
    {
        // Arrange
        var roomId = Guid.NewGuid();

        _chatRoomServiceMock
            .Setup(x => x.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Chat room with id '{roomId}' not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetById(roomId));
    }

    #endregion

    #region GetMessages Tests

    [Fact]
    public async Task GetMessages_WithExistingRoom_ReturnsPagedMessages()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var messages = new List<MessageDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Content = "Hello",
                Timestamp = DateTime.UtcNow,
                UserName = "user1",
                IsBot = false,
                ChatRoomId = roomId
            },
            new()
            {
                Id = Guid.NewGuid(),
                Content = "AAPL.US quote is $150.25 per share",
                Timestamp = DateTime.UtcNow,
                UserName = "StockBot",
                IsBot = true,
                ChatRoomId = roomId
            }
        };

        var pagedResult = new PagedResult<MessageDto>
        {
            Items = messages,
            Page = 1,
            PageSize = 50,
            TotalCount = 2
        };

        _chatRoomServiceMock
            .Setup(x => x.GetMessagesAsync(roomId, 1, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetMessages(roomId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<PagedResult<MessageDto>>>().Subject;

        response.Success.Should().BeTrue();
        var returnedPagedResult = response.Data!;
        returnedPagedResult.Items.Should().HaveCount(2);
        returnedPagedResult.Page.Should().Be(1);
        returnedPagedResult.PageSize.Should().Be(50);
        returnedPagedResult.TotalCount.Should().Be(2);

        var messageDtos = returnedPagedResult.Items.ToList();
        messageDtos[0].Content.Should().Be("Hello");
        messageDtos[0].IsBot.Should().BeFalse();
        messageDtos[1].Content.Should().Contain("AAPL.US");
        messageDtos[1].IsBot.Should().BeTrue();
    }

    [Fact]
    public async Task GetMessages_WithNonExistentRoom_ThrowsNotFoundException()
    {
        // Arrange
        var roomId = Guid.NewGuid();

        _chatRoomServiceMock
            .Setup(x => x.GetMessagesAsync(roomId, 1, 50, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Chat room with id '{roomId}' not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetMessages(roomId));
    }

    [Fact]
    public async Task GetMessages_WithEmptyRoom_ReturnsEmptyPagedResult()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var pagedResult = new PagedResult<MessageDto>
        {
            Items = new List<MessageDto>(),
            Page = 1,
            PageSize = 50,
            TotalCount = 0
        };

        _chatRoomServiceMock
            .Setup(x => x.GetMessagesAsync(roomId, 1, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetMessages(roomId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<PagedResult<MessageDto>>>().Subject;

        response.Success.Should().BeTrue();
        response.Data!.Items.Should().BeEmpty();
        response.Data.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetMessages_WithPaginationParams_UsesCorrectValues()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var pagedResult = new PagedResult<MessageDto>
        {
            Items = new List<MessageDto>(),
            Page = 2,
            PageSize = 25,
            TotalCount = 75
        };

        _chatRoomServiceMock
            .Setup(x => x.GetMessagesAsync(roomId, 2, 25, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        await _controller.GetMessages(roomId, page: 2, pageSize: 25);

        // Assert
        _chatRoomServiceMock.Verify(
            x => x.GetMessagesAsync(roomId, 2, 25, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetMessages_ReturnsCorrectPaginationMetadata()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var messages = Enumerable.Range(1, 50).Select(i => new MessageDto
        {
            Id = Guid.NewGuid(),
            Content = $"Message {i}",
            Timestamp = DateTime.UtcNow,
            UserName = "user1",
            IsBot = false,
            ChatRoomId = roomId
        }).ToList();

        var pagedResult = new PagedResult<MessageDto>
        {
            Items = messages,
            Page = 1,
            PageSize = 50,
            TotalCount = 150
        };

        _chatRoomServiceMock
            .Setup(x => x.GetMessagesAsync(roomId, 1, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetMessages(roomId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<PagedResult<MessageDto>>>().Subject;

        var returnedPagedResult = response.Data!;
        returnedPagedResult.Page.Should().Be(1);
        returnedPagedResult.PageSize.Should().Be(50);
        returnedPagedResult.TotalCount.Should().Be(150);
        returnedPagedResult.TotalPages.Should().Be(3);
        returnedPagedResult.HasPreviousPage.Should().BeFalse();
        returnedPagedResult.HasNextPage.Should().BeTrue();
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedWithRoom()
    {
        // Arrange
        var createDto = new CreateChatRoomDto
        {
            Name = "New Room",
            Description = "A new chat room"
        };

        var createdRoom = new ChatRoomDto
        {
            Id = Guid.NewGuid(),
            Name = "New Room",
            Description = "A new chat room",
            CreatedAt = DateTime.UtcNow,
            MessageCount = 0
        };

        _chatRoomServiceMock
            .Setup(x => x.CreateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdRoom);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be("GetById");

        var response = createdResult.Value.Should().BeOfType<ApiResponse<ChatRoomDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Name.Should().Be("New Room");
        response.Data.Description.Should().Be("A new chat room");
        response.Data.MessageCount.Should().Be(0);
        response.Message.Should().Contain("created successfully");
    }

    [Fact]
    public async Task Create_WithDuplicateName_ThrowsConflictException()
    {
        // Arrange
        var createDto = new CreateChatRoomDto
        {
            Name = "Existing Room",
            Description = "Description"
        };

        _chatRoomServiceMock
            .Setup(x => x.CreateAsync(createDto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConflictException($"Chat room with name '{createDto.Name}' already exists"));

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _controller.Create(createDto));
    }

    [Fact]
    public async Task Create_CallsServiceWithCorrectDto()
    {
        // Arrange
        var createDto = new CreateChatRoomDto
        {
            Name = "New Room",
            Description = "Description"
        };

        var createdRoom = new ChatRoomDto
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Description = createDto.Description,
            CreatedAt = DateTime.UtcNow,
            MessageCount = 0
        };

        _chatRoomServiceMock
            .Setup(x => x.CreateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdRoom);

        // Act
        await _controller.Create(createDto);

        // Assert
        _chatRoomServiceMock.Verify(x => x.CreateAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WithExistingRoom_ReturnsOk()
    {
        // Arrange
        var roomId = Guid.NewGuid();

        _chatRoomServiceMock
            .Setup(x => x.DeleteAsync(roomId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(roomId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse>().Subject;

        response.Success.Should().BeTrue();
        response.Message.Should().Contain("deleted successfully");
    }

    [Fact]
    public async Task Delete_WithNonExistentRoom_ThrowsNotFoundException()
    {
        // Arrange
        var roomId = Guid.NewGuid();

        _chatRoomServiceMock
            .Setup(x => x.DeleteAsync(roomId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Chat room with id '{roomId}' not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _controller.Delete(roomId));
    }

    [Fact]
    public async Task Delete_CallsServiceWithCorrectId()
    {
        // Arrange
        var roomId = Guid.NewGuid();

        _chatRoomServiceMock
            .Setup(x => x.DeleteAsync(roomId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _controller.Delete(roomId);

        // Assert
        _chatRoomServiceMock.Verify(x => x.DeleteAsync(roomId, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
