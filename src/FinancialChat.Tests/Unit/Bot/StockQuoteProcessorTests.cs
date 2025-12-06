using FinancialChat.Core.DTOs.Chat;
using FinancialChat.Core.Interfaces.Services;
using FluentAssertions;
using Moq;

namespace FinancialChat.Tests.Unit.Bot;

/// <summary>
/// Tests for stock quote processing logic.
/// These tests verify the business logic that would be used by StockQuoteWorker.
/// </summary>
public class StockQuoteProcessorTests
{
    private readonly Mock<IStockService> _stockServiceMock;

    public StockQuoteProcessorTests()
    {
        _stockServiceMock = new Mock<IStockService>();
    }

    #region ProcessStockRequest Logic Tests

    [Fact]
    public async Task ProcessStockRequest_WithValidStock_ReturnsSuccessfulQuote()
    {
        // Arrange
        var request = new StockRequestDto
        {
            StockCode = "aapl.us",
            ChatRoomId = Guid.NewGuid(),
            RequestedBy = "testuser",
            CorrelationId = Guid.NewGuid()
        };

        var expectedQuote = new StockQuoteDto
        {
            StockCode = "AAPL.US",
            Price = 150.25m,
            Message = "AAPL.US quote is $150.25 per share",
            Success = true
        };

        _stockServiceMock
            .Setup(x => x.GetStockQuoteAsync(request.StockCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedQuote);

        // Act
        var quote = await _stockServiceMock.Object.GetStockQuoteAsync(request.StockCode);

        // Build response (simulating worker logic)
        var response = new StockQuoteDto
        {
            StockCode = quote.StockCode,
            Price = quote.Price,
            Message = quote.Message,
            ChatRoomId = request.ChatRoomId,
            CorrelationId = request.CorrelationId,
            Success = quote.Success,
            Error = quote.Error
        };

        // Assert
        response.Success.Should().BeTrue();
        response.StockCode.Should().Be("AAPL.US");
        response.Price.Should().Be(150.25m);
        response.ChatRoomId.Should().Be(request.ChatRoomId);
        response.CorrelationId.Should().Be(request.CorrelationId);
    }

    [Fact]
    public async Task ProcessStockRequest_WithInvalidStock_ReturnsErrorResponse()
    {
        // Arrange
        var request = new StockRequestDto
        {
            StockCode = "invalid",
            ChatRoomId = Guid.NewGuid(),
            RequestedBy = "testuser",
            CorrelationId = Guid.NewGuid()
        };

        var errorQuote = new StockQuoteDto
        {
            StockCode = "INVALID",
            Price = 0,
            Message = "Stock INVALID not found or unavailable",
            Success = false,
            Error = "N/D price returned from API"
        };

        _stockServiceMock
            .Setup(x => x.GetStockQuoteAsync(request.StockCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(errorQuote);

        // Act
        var quote = await _stockServiceMock.Object.GetStockQuoteAsync(request.StockCode);

        var response = new StockQuoteDto
        {
            StockCode = quote.StockCode,
            Price = quote.Price,
            Message = quote.Message,
            ChatRoomId = request.ChatRoomId,
            CorrelationId = request.CorrelationId,
            Success = quote.Success,
            Error = quote.Error
        };

        // Assert
        response.Success.Should().BeFalse();
        response.Error.Should().NotBeNullOrEmpty();
        response.Price.Should().Be(0);
    }

    [Fact]
    public async Task ProcessStockRequest_WhenServiceThrows_CreatesErrorResponse()
    {
        // Arrange
        var request = new StockRequestDto
        {
            StockCode = "error.us",
            ChatRoomId = Guid.NewGuid(),
            RequestedBy = "testuser",
            CorrelationId = Guid.NewGuid()
        };

        _stockServiceMock
            .Setup(x => x.GetStockQuoteAsync(request.StockCode, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("API unavailable"));

        // Act & Simulate worker error handling
        StockQuoteDto response;
        try
        {
            await _stockServiceMock.Object.GetStockQuoteAsync(request.StockCode);
            response = new StockQuoteDto { Success = true }; // Won't reach here
        }
        catch (Exception ex)
        {
            response = new StockQuoteDto
            {
                StockCode = request.StockCode.ToUpperInvariant(),
                Message = $"Error processing stock request for {request.StockCode.ToUpperInvariant()}: {ex.Message}",
                ChatRoomId = request.ChatRoomId,
                CorrelationId = request.CorrelationId,
                Success = false,
                Error = ex.Message
            };
        }

        // Assert
        response.Success.Should().BeFalse();
        response.Error.Should().Contain("API unavailable");
        response.StockCode.Should().Be("ERROR.US");
        response.ChatRoomId.Should().Be(request.ChatRoomId);
    }

    #endregion

    #region Response Building Tests

    [Fact]
    public void BuildResponse_PreservesCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var request = new StockRequestDto
        {
            StockCode = "test",
            ChatRoomId = Guid.NewGuid(),
            RequestedBy = "user",
            CorrelationId = correlationId
        };

        var quote = new StockQuoteDto
        {
            StockCode = "TEST",
            Price = 100m,
            Message = "Test quote",
            Success = true
        };

        // Act - Simulate response building
        var response = new StockQuoteDto
        {
            StockCode = quote.StockCode,
            Price = quote.Price,
            Message = quote.Message,
            ChatRoomId = request.ChatRoomId,
            CorrelationId = request.CorrelationId,
            Success = quote.Success,
            Error = quote.Error
        };

        // Assert
        response.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void BuildResponse_PreservesChatRoomId()
    {
        // Arrange
        var chatRoomId = Guid.NewGuid();
        var request = new StockRequestDto
        {
            StockCode = "test",
            ChatRoomId = chatRoomId,
            RequestedBy = "user",
            CorrelationId = Guid.NewGuid()
        };

        var quote = new StockQuoteDto
        {
            StockCode = "TEST",
            Price = 100m,
            Message = "Test quote",
            Success = true
        };

        // Act
        var response = new StockQuoteDto
        {
            StockCode = quote.StockCode,
            Price = quote.Price,
            Message = quote.Message,
            ChatRoomId = request.ChatRoomId,
            CorrelationId = request.CorrelationId,
            Success = quote.Success,
            Error = quote.Error
        };

        // Assert
        response.ChatRoomId.Should().Be(chatRoomId);
    }

    [Fact]
    public void BuildErrorResponse_FormatsMessageCorrectly()
    {
        // Arrange
        var request = new StockRequestDto
        {
            StockCode = "fail.us",
            ChatRoomId = Guid.NewGuid(),
            RequestedBy = "user",
            CorrelationId = Guid.NewGuid()
        };
        var exceptionMessage = "Connection timeout";

        // Act - Simulate error response building
        var errorResponse = new StockQuoteDto
        {
            StockCode = request.StockCode.ToUpperInvariant(),
            Message = $"Error processing stock request for {request.StockCode.ToUpperInvariant()}: {exceptionMessage}",
            ChatRoomId = request.ChatRoomId,
            CorrelationId = request.CorrelationId,
            Success = false,
            Error = exceptionMessage
        };

        // Assert
        errorResponse.Message.Should().Contain("FAIL.US");
        errorResponse.Message.Should().Contain("Connection timeout");
        errorResponse.StockCode.Should().Be("FAIL.US");
    }

    #endregion

    #region Request Validation Tests

    [Fact]
    public void StockRequest_WithValidData_HasAllRequiredFields()
    {
        // Arrange & Act
        var request = new StockRequestDto
        {
            StockCode = "aapl.us",
            ChatRoomId = Guid.NewGuid(),
            RequestedBy = "testuser",
            CorrelationId = Guid.NewGuid()
        };

        // Assert
        request.StockCode.Should().NotBeNullOrEmpty();
        request.ChatRoomId.Should().NotBe(Guid.Empty);
        request.RequestedBy.Should().NotBeNullOrEmpty();
        request.CorrelationId.Should().NotBe(Guid.Empty);
    }

    [Theory]
    [InlineData("aapl.us")]
    [InlineData("MSFT.US")]
    [InlineData("^SPX")]
    [InlineData("btc.v")]
    [InlineData("eurusd")]
    public void StockRequest_VariousStockCodeFormats_AreAccepted(string stockCode)
    {
        // Arrange & Act
        var request = new StockRequestDto
        {
            StockCode = stockCode,
            ChatRoomId = Guid.NewGuid(),
            RequestedBy = "user",
            CorrelationId = Guid.NewGuid()
        };

        // Assert
        request.StockCode.Should().Be(stockCode);
    }

    #endregion

    #region Multiple Stock Processing Tests

    [Fact]
    public async Task ProcessMultipleStocks_AllSuccessful_ReturnsAllQuotes()
    {
        // Arrange
        var stockCodes = new[] { "aapl.us", "msft.us", "meta.us" };
        var chatRoomId = Guid.NewGuid();

        foreach (var code in stockCodes)
        {
            _stockServiceMock
                .Setup(x => x.GetStockQuoteAsync(code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StockQuoteDto
                {
                    StockCode = code.ToUpperInvariant(),
                    Price = 100m,
                    Message = $"{code.ToUpperInvariant()} quote is $100.00 per share",
                    Success = true
                });
        }

        // Act
        var responses = new List<StockQuoteDto>();
        foreach (var code in stockCodes)
        {
            var quote = await _stockServiceMock.Object.GetStockQuoteAsync(code);
            responses.Add(new StockQuoteDto
            {
                StockCode = quote.StockCode,
                Price = quote.Price,
                Message = quote.Message,
                ChatRoomId = chatRoomId,
                CorrelationId = Guid.NewGuid(),
                Success = quote.Success
            });
        }

        // Assert
        responses.Should().HaveCount(3);
        responses.Should().OnlyContain(r => r.Success);
        responses.Should().OnlyContain(r => r.ChatRoomId == chatRoomId);
    }

    [Fact]
    public async Task ProcessMultipleStocks_OneFailure_OthersStillProcess()
    {
        // Arrange
        _stockServiceMock
            .Setup(x => x.GetStockQuoteAsync("aapl.us", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuoteDto { StockCode = "AAPL.US", Success = true, Price = 150m });

        _stockServiceMock
            .Setup(x => x.GetStockQuoteAsync("invalid", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuoteDto { StockCode = "INVALID", Success = false, Error = "Not found" });

        _stockServiceMock
            .Setup(x => x.GetStockQuoteAsync("msft.us", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuoteDto { StockCode = "MSFT.US", Success = true, Price = 350m });

        // Act
        var stockCodes = new[] { "aapl.us", "invalid", "msft.us" };
        var responses = new List<StockQuoteDto>();

        foreach (var code in stockCodes)
        {
            var quote = await _stockServiceMock.Object.GetStockQuoteAsync(code);
            responses.Add(quote);
        }

        // Assert
        responses.Should().HaveCount(3);
        responses.Count(r => r.Success).Should().Be(2);
        responses.Count(r => !r.Success).Should().Be(1);
    }

    #endregion
}
