using FinancialChat.Core.DTOs.Chat;

namespace FinancialChat.Core.Interfaces.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(MessageDto message);
    Task ReceiveStockQuote(StockQuoteDto quote);
    Task UserJoined(string userName);
    Task UserLeft(string userName);
    Task Error(string message);
}
