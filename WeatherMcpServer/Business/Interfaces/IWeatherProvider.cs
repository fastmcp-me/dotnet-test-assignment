using WeatherMcpServer.Business.Models;

namespace WeatherMcpServer.Business.Interfaces;

public interface IWeatherProvider
{
    Task<CurrentWeatherData> GetCurrentWeatherAsync(string city, string? countryCode = null, CancellationToken cancellationToken = default);
    Task<WeatherForecastData> GetWeatherForecastAsync(string city, string? countryCode = null, int days = 3, CancellationToken cancellationToken = default);
    Task<WeatherAlertsData> GetWeatherAlertsAsync(string city, string? countryCode = null, CancellationToken cancellationToken = default);
}