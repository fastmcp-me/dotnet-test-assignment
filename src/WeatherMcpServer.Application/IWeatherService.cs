using WeatherMcpServer.Domain.LocationAggregate;

namespace WeatherMcpServer.Application;

public interface IWeatherService
{
    Task<LocationWeather> GetLocationWeatherAsync(Location location, int forecastDays = 3);
}
