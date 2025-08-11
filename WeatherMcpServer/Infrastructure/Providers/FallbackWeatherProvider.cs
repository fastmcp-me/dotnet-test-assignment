using WeatherMcpServer.Application.Interfaces;
using WeatherMcpServer.Domain;

namespace WeatherMcpServer.Infrastructure.Providers;

public class FallbackWeatherProvider : IWeatherProvider
{
    public string Name => "MockProvider";
    public int Weight => 0;
    public Task<WeatherResult> GetCurrentWeatherAsync(string location, CancellationToken ct = default)
    {
        var result = new WeatherResult
        {
            ProviderName = Name,
            Location = location,
            Timestamp = DateTimeOffset.UtcNow,
            Description = "Clear (mock)",
            TemperatureC = 20.0
        };
        return Task.FromResult(result);
    }

    public Task<IEnumerable<WeatherResult>> GetForecastAsync(string location, int days = 3, CancellationToken ct = default)
    {
        var list = Enumerable.Range(0, days).Select(i =>
            new WeatherResult
            {
                ProviderName = Name,
                Location = location,
                Timestamp = DateTimeOffset.UtcNow.AddDays(i),
                Description = "Partly cloudy (mock)",
                TemperatureC = 20 + i
            });
        return Task.FromResult(list);
    }
}
