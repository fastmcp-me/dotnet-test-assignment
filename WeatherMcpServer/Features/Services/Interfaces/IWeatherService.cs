using WeatherMcpServer.Features.Services.DTOs.Interfaces;

namespace WeatherMcpServer.Features.Services.Interfaces;

/// <summary>
/// Represents a service for getting weather information.
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// Gets the current weather for a specified city.
    /// </summary>
    /// <param name="city">The city name.</param>
    /// <param name="countryCode">The country code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The current weather data.</returns>
    Task<ICurrentWeatherDto> GetCurrentWeather(string city, string? countryCode, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the weather forecast for a specified city.
    /// </summary>
    /// <param name="city">The city name.</param>
    /// <param name="countryCode">The country code.</param>
    /// <param name="days">The number of days to forecast.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The weather forecast data.</returns>
    Task<IWeatherForecastDto> GetWeatherForecast(string city, string? countryCode, int days, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the weather alerts for a specified city.
    /// </summary>
    /// <param name="city">The city name.</param>
    /// <param name="countryCode">The country code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The weather alert data.</returns>
    Task<IWeatherAlertDto?> GetWeatherAlerts(string city, string? countryCode, CancellationToken cancellationToken);
}
