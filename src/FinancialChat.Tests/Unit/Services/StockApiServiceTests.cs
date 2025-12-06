using System.Net;
using FinancialChat.Core.Settings;
using FinancialChat.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace FinancialChat.Tests.Unit.Services;

public class StockApiServiceTests
{
    private readonly Mock<ILogger<StockApiService>> _loggerMock;
    private readonly StooqApiSettings _settings;

    public StockApiServiceTests()
    {
        _loggerMock = new Mock<ILogger<StockApiService>>();
        _settings = new StooqApiSettings
        {
            BaseUrl = "https://stooq.com/q/l/"
        };
    }

    private StockApiService CreateService(HttpClient httpClient)
    {
        var options = new Mock<IOptions<StooqApiSettings>>();
        options.Setup(x => x.Value).Returns(_settings);

        return new StockApiService(httpClient, options.Object, _loggerMock.Object);
    }

    private HttpClient CreateMockHttpClient(string responseContent, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(responseContent)
            });

        return new HttpClient(handlerMock.Object);
    }

    [Fact]
    public async Task GetStockQuoteAsync_WithValidResponse_ReturnsSuccessfulQuote()
    {
        // Arrange
        var csvResponse = """
            Symbol,Date,Time,Open,High,Low,Close,Volume
            AAPL.US,2024-01-15,22:00:00,150.12,151.47,149.89,150.25,45678900
            """;

        var httpClient = CreateMockHttpClient(csvResponse);
        var service = CreateService(httpClient);

        // Act
        var result = await service.GetStockQuoteAsync("aapl.us");

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.StockCode.Should().Be("AAPL.US");
        result.Price.Should().Be(150.25m);
        result.Message.Should().Be("AAPL.US quote is $150.25 per share");
        result.Error.Should().BeNull();
    }

    [Fact]
    public async Task GetStockQuoteAsync_WithNoDataResponse_ReturnsError()
    {
        // Arrange
        var csvResponse = """
            Symbol,Date,Time,Open,High,Low,Close,Volume
            INVALID.US,N/D,N/D,N/D,N/D,N/D,N/D,N/D
            """;

        var httpClient = CreateMockHttpClient(csvResponse);
        var service = CreateService(httpClient);

        // Act
        var result = await service.GetStockQuoteAsync("invalid.us");

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task GetStockQuoteAsync_WithInvalidCsvFormat_ReturnsError()
    {
        // Arrange
        var csvResponse = "Invalid CSV Content";

        var httpClient = CreateMockHttpClient(csvResponse);
        var service = CreateService(httpClient);

        // Act
        var result = await service.GetStockQuoteAsync("aapl.us");

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetStockQuoteAsync_WithDecimalPrice_ParsesCorrectly()
    {
        // Arrange
        var csvResponse = """
            Symbol,Date,Time,Open,High,Low,Close,Volume
            MSFT.US,2024-01-15,22:00:00,375.50,378.99,374.25,377.85,12345678
            """;

        var httpClient = CreateMockHttpClient(csvResponse);
        var service = CreateService(httpClient);

        // Act
        var result = await service.GetStockQuoteAsync("msft.us");

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.StockCode.Should().Be("MSFT.US");
        result.Price.Should().Be(377.85m);
    }

    [Fact]
    public async Task GetStockQuoteAsync_StockCodeIsUppercased()
    {
        // Arrange
        var csvResponse = """
            Symbol,Date,Time,Open,High,Low,Close,Volume
            GOOGL.US,2024-01-15,22:00:00,140.00,142.50,139.75,141.25,9876543
            """;

        var httpClient = CreateMockHttpClient(csvResponse);
        var service = CreateService(httpClient);

        // Act
        var result = await service.GetStockQuoteAsync("googl.us");

        // Assert
        result.StockCode.Should().Be("GOOGL.US");
    }

    [Fact]
    public async Task GetStockQuoteAsync_WithHttpError_ReturnsErrorResponse()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var httpClient = new HttpClient(handlerMock.Object);
        var service = CreateService(httpClient);

        // Act
        var result = await service.GetStockQuoteAsync("aapl.us");

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Error fetching stock quote");
    }

    [Fact]
    public async Task GetStockQuoteAsync_WithEmptyResponse_ReturnsError()
    {
        // Arrange
        var csvResponse = "";

        var httpClient = CreateMockHttpClient(csvResponse);
        var service = CreateService(httpClient);

        // Act
        var result = await service.GetStockQuoteAsync("aapl.us");

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task GetStockQuoteAsync_FormatsMessageCorrectly()
    {
        // Arrange
        var csvResponse = """
            Symbol,Date,Time,Open,High,Low,Close,Volume
            TSLA.US,2024-01-15,22:00:00,245.00,250.00,243.50,248.75,55555555
            """;

        var httpClient = CreateMockHttpClient(csvResponse);
        var service = CreateService(httpClient);

        // Act
        var result = await service.GetStockQuoteAsync("tsla.us");

        // Assert
        result.Message.Should().Be("TSLA.US quote is $248.75 per share");
    }
}
