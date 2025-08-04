using Domain.Common.DTOs.OpenWeatherMap;

namespace Domain.Common.Weathers;

public interface IWeatherClient
{
    Task<WeatherDto> GetCurrentAsync(string city, string? country, CancellationToken ct);
    Task<string?> GetAlertsAsync(string city, string? country, CancellationToken ct);
    Task<IEnumerable<ForecastDto>> GetForecastAsync(string city, string? country, int days, CancellationToken ct);
}