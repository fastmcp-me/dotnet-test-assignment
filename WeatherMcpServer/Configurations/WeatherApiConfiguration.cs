namespace WeatherMcpServer.Configurations;

/// <summary>
/// Represents the configuration for the weather API.
/// </summary>
public class WeatherApiConfiguration
{
    /// <summary>
    /// Gets or sets the API key.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL of the API.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the units for the measurements (e.g., metric, imperial).
    /// </summary>
    public string Units { get; set; } = string.Empty;
}
