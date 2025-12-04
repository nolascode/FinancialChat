namespace FinancialChat.Core.Settings;

/// <summary>
/// Configuration settings for the external Stooq API.
/// </summary>
public class StooqApiSettings
{
    /// <summary>
    /// Gets or sets the base URL for the Stooq stock data API.
    /// </summary>
    public string BaseUrl { get; set; } = "https://stooq.com/q/l/";
}
