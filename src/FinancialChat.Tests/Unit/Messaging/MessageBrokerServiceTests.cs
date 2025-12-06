using System.Text.Json;
using FinancialChat.Core.DTOs.Chat;
using FluentAssertions;

namespace FinancialChat.Tests.Unit.Messaging;

/// <summary>
/// Tests for message broker DTOs and serialization.
/// Note: RabbitMqService integration tests require a running RabbitMQ instance.
/// These unit tests verify DTO contracts and serialization behavior.
/// </summary>
public class MessageBrokerServiceTests
{
    #region StockRequestDto Serialization Tests

    [Fact]
    public void StockRequestDto_SerializesToJson_Correctly()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var chatRoomId = Guid.NewGuid();
        var request = new StockRequestDto
        {
            StockCode = "aapl.us",
            ChatRoomId = chatRoomId,
            RequestedBy = "testuser",
            CorrelationId = correlationId
        };

        // Act
        var json = JsonSerializer.Serialize(request);
        var deserialized = JsonSerializer.Deserialize<StockRequestDto>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.StockCode.Should().Be("aapl.us");
        deserialized.ChatRoomId.Should().Be(chatRoomId);
        deserialized.RequestedBy.Should().Be("testuser");
        deserialized.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void StockRequestDto_DeserializesFromJson_CaseInsensitive()
    {
        // Arrange
        var json = """
        {
            "stockCode": "msft.us",
            "chatRoomId": "00000000-0000-0000-0000-000000000001",
            "requestedBy": "user1",
            "correlationId": "00000000-0000-0000-0000-000000000002"
        }
        """;

        // Act
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var request = JsonSerializer.Deserialize<StockRequestDto>(json, options);

        // Assert
        request.Should().NotBeNull();
        request!.StockCode.Should().Be("msft.us");
        request.RequestedBy.Should().Be("user1");
    }

    [Fact]
    public void StockRequestDto_AllPropertiesHaveValues()
    {
        // Arrange & Act
        var request = new StockRequestDto
        {
            StockCode = "test",
            ChatRoomId = Guid.NewGuid(),
            RequestedBy = "user",
            CorrelationId = Guid.NewGuid()
        };

        // Assert
        request.StockCode.Should().NotBeNullOrEmpty();
        request.ChatRoomId.Should().NotBe(Guid.Empty);
        request.RequestedBy.Should().NotBeNullOrEmpty();
        request.CorrelationId.Should().NotBe(Guid.Empty);
    }

    #endregion

    #region StockQuoteDto Serialization Tests

    [Fact]
    public void StockQuoteDto_SerializesToJson_Correctly()
    {
        // Arrange
        var response = new StockQuoteDto
        {
            StockCode = "AAPL.US",
            Price = 150.25m,
            Message = "AAPL.US quote is $150.25 per share",
            ChatRoomId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            Success = true,
            Error = null
        };

        // Act
        var json = JsonSerializer.Serialize(response);
        var deserialized = JsonSerializer.Deserialize<StockQuoteDto>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.StockCode.Should().Be("AAPL.US");
        deserialized.Price.Should().Be(150.25m);
        deserialized.Message.Should().Contain("$150.25");
        deserialized.Success.Should().BeTrue();
    }

    [Fact]
    public void StockQuoteDto_WithError_SerializesCorrectly()
    {
        // Arrange
        var response = new StockQuoteDto
        {
            StockCode = "INVALID",
            Price = 0,
            Message = "Stock INVALID not found",
            ChatRoomId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            Success = false,
            Error = "Stock not found in Stooq API"
        };

        // Act
        var json = JsonSerializer.Serialize(response);
        var deserialized = JsonSerializer.Deserialize<StockQuoteDto>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Success.Should().BeFalse();
        deserialized.Error.Should().NotBeNullOrEmpty();
        deserialized.Price.Should().Be(0);
    }

    [Fact]
    public void StockQuoteDto_DeserializesFromJson_CaseInsensitive()
    {
        // Arrange
        var json = """
        {
            "stockCode": "META.US",
            "price": 325.50,
            "message": "META.US quote is $325.50 per share",
            "chatRoomId": "00000000-0000-0000-0000-000000000001",
            "correlationId": "00000000-0000-0000-0000-000000000002",
            "success": true,
            "error": null
        }
        """;

        // Act
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var response = JsonSerializer.Deserialize<StockQuoteDto>(json, options);

        // Assert
        response.Should().NotBeNull();
        response!.StockCode.Should().Be("META.US");
        response.Price.Should().Be(325.50m);
        response.Success.Should().BeTrue();
    }

    #endregion

    #region MessageDto Serialization Tests

    [Fact]
    public void MessageDto_SerializesToJson_Correctly()
    {
        // Arrange
        var dto = new MessageDto
        {
            Id = Guid.NewGuid(),
            Content = "Hello, world!",
            Timestamp = DateTime.UtcNow,
            UserName = "testuser",
            IsBot = false,
            ChatRoomId = Guid.NewGuid()
        };

        // Act
        var json = JsonSerializer.Serialize(dto);
        var deserialized = JsonSerializer.Deserialize<MessageDto>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Content.Should().Be("Hello, world!");
        deserialized.UserName.Should().Be("testuser");
        deserialized.IsBot.Should().BeFalse();
    }

    [Fact]
    public void MessageDto_BotMessage_SerializesCorrectly()
    {
        // Arrange
        var dto = new MessageDto
        {
            Id = Guid.NewGuid(),
            Content = "AAPL.US quote is $150.25 per share",
            Timestamp = DateTime.UtcNow,
            UserName = "StockBot",
            IsBot = true,
            ChatRoomId = Guid.NewGuid()
        };

        // Act
        var json = JsonSerializer.Serialize(dto);
        var deserialized = JsonSerializer.Deserialize<MessageDto>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.IsBot.Should().BeTrue();
        deserialized.UserName.Should().Be("StockBot");
    }

    #endregion

    #region Message Contract Tests

    [Fact]
    public void StockRequestDto_ToJsonFormat_MatchesExpectedSchema()
    {
        // Arrange
        var request = new StockRequestDto
        {
            StockCode = "aapl.us",
            ChatRoomId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            RequestedBy = "user",
            CorrelationId = Guid.Parse("22222222-2222-2222-2222-222222222222")
        };

        // Act
        var json = JsonSerializer.Serialize(request);

        // Assert - verify JSON structure
        json.Should().Contain("\"StockCode\"");
        json.Should().Contain("\"ChatRoomId\"");
        json.Should().Contain("\"RequestedBy\"");
        json.Should().Contain("\"CorrelationId\"");
        json.Should().Contain("aapl.us");
    }

    [Fact]
    public void StockQuoteDto_ToJsonFormat_MatchesExpectedSchema()
    {
        // Arrange
        var response = new StockQuoteDto
        {
            StockCode = "AAPL.US",
            Price = 150.25m,
            Message = "Test message",
            ChatRoomId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            Success = true
        };

        // Act
        var json = JsonSerializer.Serialize(response);

        // Assert
        json.Should().Contain("\"StockCode\"");
        json.Should().Contain("\"Price\"");
        json.Should().Contain("\"Message\"");
        json.Should().Contain("\"Success\"");
        json.Should().Contain("150.25");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void StockRequestDto_WithSpecialCharacters_SerializesCorrectly()
    {
        // Arrange
        var request = new StockRequestDto
        {
            StockCode = "^SPX",
            ChatRoomId = Guid.NewGuid(),
            RequestedBy = "user@test.com",
            CorrelationId = Guid.NewGuid()
        };

        // Act
        var json = JsonSerializer.Serialize(request);
        var deserialized = JsonSerializer.Deserialize<StockRequestDto>(json);

        // Assert
        deserialized!.StockCode.Should().Be("^SPX");
        deserialized.RequestedBy.Should().Be("user@test.com");
    }

    [Fact]
    public void StockQuoteDto_WithLargePrice_SerializesCorrectly()
    {
        // Arrange
        var response = new StockQuoteDto
        {
            StockCode = "BRK.A",
            Price = 500000.00m,
            Message = "BRK.A quote is $500000.00 per share",
            ChatRoomId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            Success = true
        };

        // Act
        var json = JsonSerializer.Serialize(response);
        var deserialized = JsonSerializer.Deserialize<StockQuoteDto>(json);

        // Assert
        deserialized!.Price.Should().Be(500000.00m);
    }

    [Fact]
    public void StockQuoteDto_WithDecimalPrecision_MaintainsPrecision()
    {
        // Arrange
        var response = new StockQuoteDto
        {
            StockCode = "TEST",
            Price = 123.456789m,
            Message = "Test",
            ChatRoomId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            Success = true
        };

        // Act
        var json = JsonSerializer.Serialize(response);
        var deserialized = JsonSerializer.Deserialize<StockQuoteDto>(json);

        // Assert
        deserialized!.Price.Should().Be(123.456789m);
    }

    #endregion
}
