using WeatherMcpServer.Domain;

namespace WeatherMcpServer.Application.Interfaces;

public interface IWeatherProvider
{
    string Name { get; }
    int Weight { get; }
    Task<WeatherResult> GetCurrentWeatherAsync(string location, CancellationToken ct = default);
    Task<IEnumerable<WeatherResult>> GetForecastAsync(string location, int days = 3, CancellationToken ct = default);
}
