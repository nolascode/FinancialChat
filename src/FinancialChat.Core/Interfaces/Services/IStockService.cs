using FinancialChat.Core.DTOs.Chat;

namespace FinancialChat.Core.Interfaces.Services;

/// <summary>
/// Defines service operations for retrieving external stock market data.
/// </summary>
public interface IStockService
{
    /// <summary>
    /// Retrieves a stock quote for a specific stock code.
    /// </summary>
    /// <param name="stockCode">The stock symbol code (e.g., AAPL.US).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The stock quote information.</returns>
    Task<StockQuoteDto> GetStockQuoteAsync(string stockCode, CancellationToken cancellationToken = default);
}
