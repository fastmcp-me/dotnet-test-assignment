using WeatherMcpServer.Domain;

namespace WeatherMcpServer.Application.Interfaces;

public interface IWeatherMediator
{
    Task<Result<WeatherResult>> GetCurrentWeatherAsync(string location, CancellationToken ct = default);
    Task<Result<IEnumerable<WeatherResult>>> GetForecastAsync(string location, int days = 3, CancellationToken ct = default);
}

