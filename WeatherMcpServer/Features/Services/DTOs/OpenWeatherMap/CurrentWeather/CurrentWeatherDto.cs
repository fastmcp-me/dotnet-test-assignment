using WeatherMcpServer.Features.Services.DTOs.Interfaces;

namespace WeatherMcpServer.Features.Services.DTOs.OpenWeatherMap.CurrentWeather;

/// <summary>
/// Represents the data transfer object for current weather.
/// </summary>
internal class CurrentWeatherDto : ICurrentWeatherDto
{
    /// <summary>
    /// Gets or sets the city name.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country code.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the temperature in the specified units (e.g., Celsius, Fahrenheit).
    /// </summary>
    public double Temperature { get; set; }

    /// <summary>
    /// Gets or sets the "feels like" temperature in the specified units (e.g., Celsius, Fahrenheit).
    /// </summary>
    public double TemperatureFeelsLike { get; set; }

    /// <summary>
    /// Gets or sets the weather description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the wind speed.
    /// </summary>
    public double WindSpeed { get; set; }

    /// <summary>
    /// Gets or sets the humidity percentage.
    /// </summary>
    public double Humidity { get; set; }

    /// <summary>
    /// Gets or sets the atmospheric pressure.
    /// </summary>
    public double Pressure { get; set; }
}
