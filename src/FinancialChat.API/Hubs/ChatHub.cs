using FinancialChat.Core.DTOs.Chat;
using FinancialChat.Core.Entities;
using FinancialChat.Core.Helpers;
using FinancialChat.Core.Interfaces.Hubs;
using FinancialChat.Core.Interfaces.Repositories;
using FinancialChat.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FinancialChat.API.Hubs;

[Authorize]
public class ChatHub : Hub<IChatClient>
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly IMessageBrokerService _messageBroker;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(
        IRepositoryManager repositoryManager,
        IMessageBrokerService messageBroker,
        ILogger<ChatHub> logger)
    {
        _repositoryManager = repositoryManager;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userName = Context.User?.Identity?.Name ?? "Anonymous";
        _logger.LogInformation("User {UserName} connected with ConnectionId {ConnectionId}", userName, Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userName = Context.User?.Identity?.Name ?? "Anonymous";
        _logger.LogInformation("User {UserName} disconnected. ConnectionId: {ConnectionId}", userName, Context.ConnectionId);

        if (exception != null)
        {
            _logger.LogError(exception, "User disconnected with error");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinRoom(Guid roomId)
    {
        var userName = Context.User?.Identity?.Name ?? "Anonymous";
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());

        _logger.LogInformation("User {UserName} joined room {RoomId}", userName, roomId);
        await Clients.Group(roomId.ToString()).UserJoined(userName);
    }

    public async Task LeaveRoom(Guid roomId)
    {
        var userName = Context.User?.Identity?.Name ?? "Anonymous";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());

        _logger.LogInformation("User {UserName} left room {RoomId}", userName, roomId);
        await Clients.Group(roomId.ToString()).UserLeft(userName);
    }

    public async Task SendMessage(Guid roomId, string content)
    {
        var userName = Context.User?.Identity?.Name ?? "Anonymous";
        var userId = GetUserId();

        _logger.LogInformation("User {UserName} sending message to room {RoomId}: {Content}", userName, roomId, content);

        // Check if it's a stock command
        if (StockCommandParser.TryParse(content, out var stockCodes))
        {
            await HandleStockCommand(roomId, stockCodes, userName);
            return;
        }

        // Regular message - save to database
        var message = new Message
        {
            Id = Guid.NewGuid(),
            Content = content,
            Timestamp = DateTime.UtcNow,
            UserName = userName,
            UserId = userId,
            ChatRoomId = roomId,
            IsBot = false
        };

        await _repositoryManager.MessageRepository.CreateAsync(message);
        await _repositoryManager.SaveAsync();

        var messageDto = new MessageDto
        {
            Id = message.Id,
            Content = message.Content,
            Timestamp = message.Timestamp,
            UserName = message.UserName,
            IsBot = false,
            ChatRoomId = roomId
        };

        await Clients.Group(roomId.ToString()).ReceiveMessage(messageDto);
    }

    private async Task HandleStockCommand(Guid roomId, IReadOnlyList<string> stockCodes, string requestedBy)
    {
        if (stockCodes.Count == 0)
        {
            await Clients.Caller.Error("Please provide at least one stock code.");
            return;
        }

        _logger.LogInformation("Processing stock command for {StockCodes} in room {RoomId}",
            string.Join(", ", stockCodes), roomId);

        var failedCodes = new List<string>();

        foreach (var stockCode in stockCodes)
        {
            var request = new StockRequestDto
            {
                StockCode = stockCode,
                ChatRoomId = roomId,
                RequestedBy = requestedBy,
                CorrelationId = Guid.NewGuid()
            };

            try
            {
                await _messageBroker.PublishStockRequestAsync(request);
                _logger.LogInformation("Stock request published for {StockCode}", stockCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish stock request for {StockCode}", stockCode);
                failedCodes.Add(stockCode);
            }
        }

        if (failedCodes.Count > 0)
        {
            await Clients.Caller.Error($"Failed to process stock requests for: {string.Join(", ", failedCodes)}. Please try again later.");
        }
    }

    private Guid? GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? Context.User?.FindFirst("sub")?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        return null;
    }
}
