using FinancialChat.Core.Helpers;
using FluentAssertions;

namespace FinancialChat.Tests.Unit.Helpers;

public class StockCommandParserTests
{
    [Fact]
    public void TryParse_WithSingleStockCode_ReturnsTrue()
    {
        // Arrange
        var message = "/stock=aapl.us";

        // Act
        var result = StockCommandParser.TryParse(message, out var stockCodes);

        // Assert
        result.Should().BeTrue();
        stockCodes.Should().HaveCount(1);
        stockCodes[0].Should().Be("aapl.us");
    }

    [Fact]
    public void TryParse_WithMultipleStockCodes_ReturnsAllCodes()
    {
        // Arrange
        var message = "/stock=aapl.us,msft.us,meta.us";

        // Act
        var result = StockCommandParser.TryParse(message, out var stockCodes);

        // Assert
        result.Should().BeTrue();
        stockCodes.Should().HaveCount(3);
        stockCodes.Should().BeEquivalentTo(new[] { "aapl.us", "msft.us", "meta.us" });
    }

    [Fact]
    public void TryParse_WithDuplicateStockCodes_RemovesDuplicates()
    {
        // Arrange
        var message = "/stock=aapl.us,AAPL.US,msft.us,aapl.us";

        // Act
        var result = StockCommandParser.TryParse(message, out var stockCodes);

        // Assert
        result.Should().BeTrue();
        stockCodes.Should().HaveCount(2);
    }

    [Fact]
    public void TryParse_WithMoreThanMaxStockCodes_LimitsToMax()
    {
        // Arrange
        var message = "/stock=aapl.us,msft.us,meta.us,googl.us,amzn.us,tsla.us,nvda.us";

        // Act
        var result = StockCommandParser.TryParse(message, out var stockCodes);

        // Assert
        result.Should().BeTrue();
        stockCodes.Should().HaveCount(StockCommandParser.MaxStockCodes);
    }

    [Fact]
    public void TryParse_WithSpacesAroundCommas_TrimsCorrectly()
    {
        // Arrange
        var message = "/stock=aapl.us , msft.us , meta.us";

        // Act
        var result = StockCommandParser.TryParse(message, out var stockCodes);

        // Assert
        result.Should().BeTrue();
        stockCodes.Should().HaveCount(3);
        stockCodes.Should().BeEquivalentTo(new[] { "aapl.us", "msft.us", "meta.us" });
    }

    [Fact]
    public void TryParse_WithEmptyStockCode_ReturnsFalse()
    {
        // Arrange
        var message = "/stock=";

        // Act
        var result = StockCommandParser.TryParse(message, out var stockCodes);

        // Assert
        result.Should().BeFalse(); // Regex requires at least one char after =
        stockCodes.Should().BeEmpty();
    }

    [Fact]
    public void TryParse_WithOnlyCommas_ReturnsEmptyList()
    {
        // Arrange
        var message = "/stock=,,,";

        // Act
        var result = StockCommandParser.TryParse(message, out var stockCodes);

        // Assert
        result.Should().BeTrue(); // It IS a stock command
        stockCodes.Should().BeEmpty();
    }

    [Fact]
    public void TryParse_WithRegularMessage_ReturnsFalse()
    {
        // Arrange
        var message = "Hello, world!";

        // Act
        var result = StockCommandParser.TryParse(message, out var stockCodes);

        // Assert
        result.Should().BeFalse();
        stockCodes.Should().BeEmpty();
    }

    [Fact]
    public void TryParse_CaseInsensitiveCommand_IsRecognized()
    {
        // Arrange
        var message = "/STOCK=aapl.us";

        // Act
        var result = StockCommandParser.TryParse(message, out var stockCodes);

        // Assert
        result.Should().BeTrue();
        stockCodes.Should().HaveCount(1);
    }

    [Fact]
    public void TryParse_WithLeadingSpaces_IsRecognized()
    {
        // Arrange
        var message = "  /stock=aapl.us";

        // Act
        var result = StockCommandParser.TryParse(message, out var stockCodes);

        // Assert
        result.Should().BeTrue();
        stockCodes.Should().HaveCount(1);
    }

    [Fact]
    public void TryParse_WithTrailingSpaces_IsRecognized()
    {
        // Arrange
        var message = "/stock=aapl.us  ";

        // Act
        var result = StockCommandParser.TryParse(message, out var stockCodes);

        // Assert
        result.Should().BeTrue();
        stockCodes.Should().HaveCount(1);
    }

    [Fact]
    public void IsStockCommand_WithValidCommand_ReturnsTrue()
    {
        // Arrange
        var message = "/stock=aapl.us";

        // Act
        var result = StockCommandParser.IsStockCommand(message);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsStockCommand_WithRegularMessage_ReturnsFalse()
    {
        // Arrange
        var message = "Hello, world!";

        // Act
        var result = StockCommandParser.IsStockCommand(message);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsStockCommand_WithNullMessage_ReturnsFalse()
    {
        // Act
        var result = StockCommandParser.IsStockCommand(null!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsStockCommand_WithEmptyMessage_ReturnsFalse()
    {
        // Act
        var result = StockCommandParser.IsStockCommand(string.Empty);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ParseStockCodes_WithValidCodes_ReturnsAll()
    {
        // Arrange
        var codesString = "aapl.us,msft.us,meta.us";

        // Act
        var result = StockCommandParser.ParseStockCodes(codesString);

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(new[] { "aapl.us", "msft.us", "meta.us" });
    }

    [Fact]
    public void ParseStockCodes_WithEmptyString_ReturnsEmptyList()
    {
        // Act
        var result = StockCommandParser.ParseStockCodes(string.Empty);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ParseStockCodes_WithNullString_ReturnsEmptyList()
    {
        // Act
        var result = StockCommandParser.ParseStockCodes(null!);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ParseStockCodes_PreservesOrder()
    {
        // Arrange
        var codesString = "meta.us,aapl.us,msft.us";

        // Act
        var result = StockCommandParser.ParseStockCodes(codesString);

        // Assert
        result[0].Should().Be("meta.us");
        result[1].Should().Be("aapl.us");
        result[2].Should().Be("msft.us");
    }

    [Fact]
    public void TryParse_WithMixedValidAndEmptyCodes_FiltersEmpty()
    {
        // Arrange
        var message = "/stock=aapl.us,,msft.us,  ,meta.us";

        // Act
        var result = StockCommandParser.TryParse(message, out var stockCodes);

        // Assert
        result.Should().BeTrue();
        stockCodes.Should().HaveCount(3);
        stockCodes.Should().BeEquivalentTo(new[] { "aapl.us", "msft.us", "meta.us" });
    }

    [Theory]
    [InlineData("/stock=btc.v")]
    [InlineData("/stock=^spx")]
    [InlineData("/stock=eurusd")]
    [InlineData("/stock=gc.f")]
    public void TryParse_WithVariousStockFormats_IsRecognized(string message)
    {
        // Act
        var result = StockCommandParser.TryParse(message, out var stockCodes);

        // Assert
        result.Should().BeTrue();
        stockCodes.Should().HaveCount(1);
    }
}
