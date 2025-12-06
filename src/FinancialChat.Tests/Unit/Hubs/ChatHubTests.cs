using System.Security.Claims;
using FinancialChat.API.Hubs;
using FinancialChat.Core.DTOs.Chat;
using FinancialChat.Core.Entities;
using FinancialChat.Core.Interfaces.Hubs;
using FinancialChat.Core.Interfaces.Repositories;
using FinancialChat.Core.Interfaces.Services;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinancialChat.Tests.Unit.Hubs;

public class ChatHubTests
{
    private readonly Mock<IRepositoryManager> _repositoryManagerMock;
    private readonly Mock<IMessageRepository> _messageRepositoryMock;
    private readonly Mock<IMessageBrokerService> _messageBrokerMock;
    private readonly Mock<ILogger<ChatHub>> _loggerMock;
    private readonly Mock<IHubCallerClients<IChatClient>> _clientsMock;
    private readonly Mock<IChatClient> _callerClientMock;
    private readonly Mock<IChatClient> _groupClientMock;
    private readonly Mock<HubCallerContext> _contextMock;
    private readonly Mock<IGroupManager> _groupsMock;
    private readonly ChatHub _hub;

    public ChatHubTests()
    {
        _repositoryManagerMock = new Mock<IRepositoryManager>();
        _messageRepositoryMock = new Mock<IMessageRepository>();
        _messageBrokerMock = new Mock<IMessageBrokerService>();
        _loggerMock = new Mock<ILogger<ChatHub>>();
        _clientsMock = new Mock<IHubCallerClients<IChatClient>>();
        _callerClientMock = new Mock<IChatClient>();
        _groupClientMock = new Mock<IChatClient>();
        _contextMock = new Mock<HubCallerContext>();
        _groupsMock = new Mock<IGroupManager>();

        _repositoryManagerMock
            .Setup(x => x.MessageRepository)
            .Returns(_messageRepositoryMock.Object);

        // Setup default Clients behavior
        _clientsMock.Setup(x => x.Caller).Returns(_callerClientMock.Object);
        _clientsMock.Setup(x => x.Group(It.IsAny<string>())).Returns(_groupClientMock.Object);

        // Setup default Context
        _contextMock.Setup(x => x.ConnectionId).Returns("test-connection-id");

        _hub = new ChatHub(
            _repositoryManagerMock.Object,
            _messageBrokerMock.Object,
            _loggerMock.Object);

        // Inject mocked properties using reflection
        SetHubClients(_hub, _clientsMock.Object);
        SetHubContext(_hub, _contextMock.Object);
        SetHubGroups(_hub, _groupsMock.Object);
    }

    private void SetHubClients(ChatHub hub, IHubCallerClients<IChatClient> clients)
    {
        var property = typeof(Hub<IChatClient>).GetProperty("Clients",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        property?.SetValue(hub, clients);
    }

    private void SetHubContext(ChatHub hub, HubCallerContext context)
    {
        var property = typeof(Hub).GetProperty("Context",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        property?.SetValue(hub, context);
    }

    private void SetHubGroups(ChatHub hub, IGroupManager groups)
    {
        var property = typeof(Hub).GetProperty("Groups",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        property?.SetValue(hub, groups);
    }

    private void SetupAuthenticatedUser(string userName, Guid? userId = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userName)
        };

        if (userId.HasValue)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()));
        }

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _contextMock.Setup(x => x.User).Returns(principal);
    }

    #region JoinRoom Tests

    [Fact]
    public async Task JoinRoom_AddsUserToGroup()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        SetupAuthenticatedUser("testuser");

        _groupsMock
            .Setup(x => x.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _groupClientMock
            .Setup(x => x.UserJoined(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _hub.JoinRoom(roomId);

        // Assert
        _groupsMock.Verify(
            x => x.AddToGroupAsync("test-connection-id", roomId.ToString(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task JoinRoom_NotifiesGroupOfUserJoined()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        SetupAuthenticatedUser("testuser");

        _groupsMock
            .Setup(x => x.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _groupClientMock
            .Setup(x => x.UserJoined(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _hub.JoinRoom(roomId);

        // Assert
        _groupClientMock.Verify(x => x.UserJoined("testuser"), Times.Once);
    }

    #endregion

    #region LeaveRoom Tests

    [Fact]
    public async Task LeaveRoom_RemovesUserFromGroup()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        SetupAuthenticatedUser("testuser");

        _groupsMock
            .Setup(x => x.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _groupClientMock
            .Setup(x => x.UserLeft(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _hub.LeaveRoom(roomId);

        // Assert
        _groupsMock.Verify(
            x => x.RemoveFromGroupAsync("test-connection-id", roomId.ToString(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LeaveRoom_NotifiesGroupOfUserLeft()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        SetupAuthenticatedUser("testuser");

        _groupsMock
            .Setup(x => x.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _groupClientMock
            .Setup(x => x.UserLeft(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _hub.LeaveRoom(roomId);

        // Assert
        _groupClientMock.Verify(x => x.UserLeft("testuser"), Times.Once);
    }

    #endregion

    #region SendMessage Tests - Regular Messages

    [Fact]
    public async Task SendMessage_WithRegularMessage_SavesMessageToDatabase()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        SetupAuthenticatedUser("testuser", userId);

        Message? capturedMessage = null;

        _messageRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .Callback<Message, CancellationToken>((msg, _) => capturedMessage = msg)
            .ReturnsAsync((Message msg, CancellationToken _) => msg);

        _repositoryManagerMock
            .Setup(x => x.SaveAsync())
            .Returns(Task.CompletedTask);

        _groupClientMock
            .Setup(x => x.ReceiveMessage(It.IsAny<MessageDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _hub.SendMessage(roomId, "Hello, world!");

        // Assert
        capturedMessage.Should().NotBeNull();
        capturedMessage!.Content.Should().Be("Hello, world!");
        capturedMessage.UserName.Should().Be("testuser");
        capturedMessage.ChatRoomId.Should().Be(roomId);
        capturedMessage.IsBot.Should().BeFalse();
        capturedMessage.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task SendMessage_WithRegularMessage_BroadcastsToGroup()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        SetupAuthenticatedUser("testuser");

        MessageDto? capturedDto = null;

        _messageRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Message msg, CancellationToken _) => msg);

        _repositoryManagerMock
            .Setup(x => x.SaveAsync())
            .Returns(Task.CompletedTask);

        _groupClientMock
            .Setup(x => x.ReceiveMessage(It.IsAny<MessageDto>()))
            .Callback<MessageDto>(dto => capturedDto = dto)
            .Returns(Task.CompletedTask);

        // Act
        await _hub.SendMessage(roomId, "Hello, world!");

        // Assert
        _clientsMock.Verify(x => x.Group(roomId.ToString()), Times.Once);
        capturedDto.Should().NotBeNull();
        capturedDto!.Content.Should().Be("Hello, world!");
        capturedDto.UserName.Should().Be("testuser");
        capturedDto.IsBot.Should().BeFalse();
    }

    [Fact]
    public async Task SendMessage_SetsTimestampToUtcNow()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        SetupAuthenticatedUser("testuser");

        Message? capturedMessage = null;

        _messageRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .Callback<Message, CancellationToken>((msg, _) => capturedMessage = msg)
            .ReturnsAsync((Message msg, CancellationToken _) => msg);

        _repositoryManagerMock
            .Setup(x => x.SaveAsync())
            .Returns(Task.CompletedTask);

        _groupClientMock
            .Setup(x => x.ReceiveMessage(It.IsAny<MessageDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var beforeCall = DateTime.UtcNow;
        await _hub.SendMessage(roomId, "Hello!");
        var afterCall = DateTime.UtcNow;

        // Assert
        capturedMessage.Should().NotBeNull();
        capturedMessage!.Timestamp.Should().BeOnOrAfter(beforeCall);
        capturedMessage.Timestamp.Should().BeOnOrBefore(afterCall);
    }

    #endregion

    #region SendMessage Tests - Stock Commands

    [Fact]
    public async Task SendMessage_WithStockCommand_PublishesToMessageBroker()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        SetupAuthenticatedUser("testuser");

        StockRequestDto? capturedRequest = null;

        _messageBrokerMock
            .Setup(x => x.PublishStockRequestAsync(It.IsAny<StockRequestDto>(), It.IsAny<CancellationToken>()))
            .Callback<StockRequestDto, CancellationToken>((req, _) => capturedRequest = req)
            .Returns(Task.CompletedTask);

        // Act
        await _hub.SendMessage(roomId, "/stock=aapl.us");

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.StockCode.Should().Be("aapl.us");
        capturedRequest.ChatRoomId.Should().Be(roomId);
        capturedRequest.RequestedBy.Should().Be("testuser");
    }

    [Fact]
    public async Task SendMessage_WithStockCommand_DoesNotSaveToDatabase()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        SetupAuthenticatedUser("testuser");

        _messageBrokerMock
            .Setup(x => x.PublishStockRequestAsync(It.IsAny<StockRequestDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _hub.SendMessage(roomId, "/stock=aapl.us");

        // Assert
        _messageRepositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _repositoryManagerMock.Verify(x => x.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task SendMessage_WithMultipleStockCodes_PublishesEachCode()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        SetupAuthenticatedUser("testuser");

        var capturedRequests = new List<StockRequestDto>();

        _messageBrokerMock
            .Setup(x => x.PublishStockRequestAsync(It.IsAny<StockRequestDto>(), It.IsAny<CancellationToken>()))
            .Callback<StockRequestDto, CancellationToken>((req, _) => capturedRequests.Add(req))
            .Returns(Task.CompletedTask);

        // Act
        await _hub.SendMessage(roomId, "/stock=aapl.us,msft.us,meta.us");

        // Assert
        capturedRequests.Should().HaveCount(3);
        capturedRequests.Select(r => r.StockCode).Should().BeEquivalentTo(new[] { "aapl.us", "msft.us", "meta.us" });
    }

    [Fact]
    public async Task SendMessage_WithEmptyStockCodes_SendsErrorToCaller()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        SetupAuthenticatedUser("testuser");

        _callerClientMock
            .Setup(x => x.Error(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act - /stock=,,, is recognized as stock command but with empty codes after filtering
        await _hub.SendMessage(roomId, "/stock=,,,");

        // Assert
        _callerClientMock.Verify(
            x => x.Error(It.Is<string>(s => s.Contains("at least one stock code"))),
            Times.Once);
    }

    [Fact]
    public async Task SendMessage_WhenBrokerFails_SendsErrorToCaller()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        SetupAuthenticatedUser("testuser");

        _messageBrokerMock
            .Setup(x => x.PublishStockRequestAsync(It.IsAny<StockRequestDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Broker unavailable"));

        _callerClientMock
            .Setup(x => x.Error(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _hub.SendMessage(roomId, "/stock=aapl.us");

        // Assert
        _callerClientMock.Verify(
            x => x.Error(It.Is<string>(s => s.Contains("Failed to process"))),
            Times.Once);
    }

    #endregion

    #region Anonymous User Tests

    [Fact]
    public async Task SendMessage_WithoutAuthenticatedUser_UsesAnonymous()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        _contextMock.Setup(x => x.User).Returns((ClaimsPrincipal?)null);

        Message? capturedMessage = null;

        _messageRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .Callback<Message, CancellationToken>((msg, _) => capturedMessage = msg)
            .ReturnsAsync((Message msg, CancellationToken _) => msg);

        _repositoryManagerMock
            .Setup(x => x.SaveAsync())
            .Returns(Task.CompletedTask);

        _groupClientMock
            .Setup(x => x.ReceiveMessage(It.IsAny<MessageDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _hub.SendMessage(roomId, "Hello!");

        // Assert
        capturedMessage.Should().NotBeNull();
        capturedMessage!.UserName.Should().Be("Anonymous");
        capturedMessage.UserId.Should().BeNull();
    }

    #endregion
}
