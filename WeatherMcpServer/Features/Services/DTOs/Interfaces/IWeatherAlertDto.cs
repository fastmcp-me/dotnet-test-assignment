namespace WeatherMcpServer.Features.Services.DTOs.Interfaces;

/// <summary>
/// Represents a weather alert.
/// </summary>
public interface IWeatherAlertDto
{
    /// <summary>
    /// Gets or sets the type of the alert.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the severity of the alert.
    /// </summary>
    public string Severity { get; set; }

    /// <summary>
    /// Gets or sets the certainty of the alert.
    /// </summary>
    public string Certainty { get; set; }

    /// <summary>
    /// Gets or sets the description of the alert.
    /// </summary>
    public string Description { get; set; }
}
