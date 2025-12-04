using System.Text.RegularExpressions;

namespace FinancialChat.Core.Helpers;

public static class StockCommandParser
{
    private static readonly Regex StockCommandRegex = new(@"^/stock=(.+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public const int MaxStockCodes = 5;

    /// <summary>
    /// Tries to parse a stock command from a message.
    /// Returns true if the message is a stock command, false otherwise.
    /// </summary>
    public static bool TryParse(string message, out IReadOnlyList<string> stockCodes)
    {
        stockCodes = Array.Empty<string>();

        if (string.IsNullOrWhiteSpace(message))
            return false;

        var match = StockCommandRegex.Match(message.Trim());
        if (!match.Success)
            return false;

        var codesString = match.Groups[1].Value;
        var codes = ParseStockCodes(codesString);

        if (codes.Count == 0)
        {
            stockCodes = Array.Empty<string>();
            return true; // It IS a stock command, but with no valid codes
        }

        stockCodes = codes;
        return true;
    }

    /// <summary>
    /// Parses a comma-separated list of stock codes.
    /// Removes duplicates (case-insensitive) and limits to MaxStockCodes.
    /// </summary>
    public static IReadOnlyList<string> ParseStockCodes(string codesString)
    {
        if (string.IsNullOrWhiteSpace(codesString))
            return Array.Empty<string>();

        return codesString
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(MaxStockCodes)
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Checks if a message is a stock command.
    /// </summary>
    public static bool IsStockCommand(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return false;

        return StockCommandRegex.IsMatch(message.Trim());
    }
}
