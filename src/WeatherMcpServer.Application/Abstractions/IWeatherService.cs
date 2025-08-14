using WeatherMcpServer.Domain.LocationAggregate;

namespace WeatherMcpServer.Application.Abstractions;

public interface IWeatherService
{
    Task<LocationWeather> GetLocationWeatherAsync(Location location, int forecastDays = 3);
}
