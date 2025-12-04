using System.Globalization;
using FinancialChat.Core.DTOs.Chat;
using FinancialChat.Core.Interfaces.Services;
using FinancialChat.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FinancialChat.Infrastructure.Services;

public class StockApiService : IStockService
{
    private readonly HttpClient _httpClient;
    private readonly StooqApiSettings _settings;
    private readonly ILogger<StockApiService> _logger;

    public StockApiService(
        HttpClient httpClient,
        IOptions<StooqApiSettings> settings,
        ILogger<StockApiService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<StockQuoteDto> GetStockQuoteAsync(string stockCode, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"{_settings.BaseUrl}?s={stockCode}&f=sd2t2ohlcv&h&e=csv";
            _logger.LogInformation("Fetching stock quote for {StockCode} from {Url}", stockCode, url);

            var response = await _httpClient.GetStringAsync(url, cancellationToken);
            return ParseCsvResponse(response, stockCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching stock quote for {StockCode}", stockCode);
            return new StockQuoteDto
            {
                StockCode = stockCode.ToUpperInvariant(),
                Success = false,
                Error = $"Error fetching stock quote: {ex.Message}",
                Message = $"Unable to retrieve quote for {stockCode.ToUpperInvariant()}"
            };
        }
    }

    private StockQuoteDto ParseCsvResponse(string csvContent, string stockCode)
    {
        try
        {
            var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length < 2)
            {
                return CreateErrorResponse(stockCode, "Invalid response from stock API");
            }

            // Skip header line, get data line
            var dataLine = lines[1];
            var values = dataLine.Split(',');

            if (values.Length < 7)
            {
                return CreateErrorResponse(stockCode, "Invalid data format from stock API");
            }

            var symbol = values[0].Trim().ToUpperInvariant();
            var closePrice = values[6].Trim();

            // Check for N/D (No Data)
            if (closePrice.Equals("N/D", StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrWhiteSpace(closePrice))
            {
                return CreateErrorResponse(stockCode, $"Stock {symbol} not found or no data available");
            }

            if (!decimal.TryParse(closePrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
            {
                return CreateErrorResponse(stockCode, $"Invalid price data for {symbol}");
            }

            return new StockQuoteDto
            {
                StockCode = symbol,
                Price = price,
                Success = true,
                Message = $"{symbol} quote is ${price:F2} per share"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing CSV response for {StockCode}", stockCode);
            return CreateErrorResponse(stockCode, "Error parsing stock data");
        }
    }

    private static StockQuoteDto CreateErrorResponse(string stockCode, string error)
    {
        return new StockQuoteDto
        {
            StockCode = stockCode.ToUpperInvariant(),
            Success = false,
            Error = error,
            Message = error
        };
    }
}
