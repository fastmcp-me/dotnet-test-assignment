using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using WeatherMcpServer.Domain.LocationAggregate;
using WeatherMcpServer.Infrastructure.OpenWeather;
using Xunit;

namespace IntegrationTests.Infrastructure;

public class OpenWeatherProviderTests : TestBase
{
    [Fact]
    public async Task GetCoordinatesAsync_ShouldReturnCoordinates_ForKnownCity()
    {
        var provider = new OpenWeatherProvider(
            _httpClient,
            _configuration, 
            NullLogger<OpenWeatherProvider>.Instance);

        var coords = await provider.GetCoordinatesAsync(Location.Create("London", "GB"));

        Assert.NotNull(coords);
        Assert.InRange(coords.Value.lat, -90, 90);
        Assert.InRange(coords.Value.lon, -180, 180);
    }


    [Fact]
    public async Task GetCurrentWeatherAsync_ShouldReturnWeather_ForKnownCity()
    {
        var provider = new OpenWeatherProvider(
            _httpClient,
            _configuration,
            NullLogger<OpenWeatherProvider>.Instance);

        var weatherInfo = await provider.GetCurrentWeatherAsync(Location.Create("Moscow", "RU"));

        Assert.NotNull(weatherInfo);
        Assert.False(string.IsNullOrWhiteSpace(weatherInfo.Description), "Description should not be empty.");
        Assert.InRange(weatherInfo.TemperatureCelsius, -50, 50); 
        Assert.InRange(weatherInfo.Humidity, 0, 100);
    }


    [Fact]
    public async Task GetWeatherForecastAsync_ShouldReturnWeatherForecast_ForKnownCity()
    {
        int numDays = 3;

        var provider = new OpenWeatherProvider(
            _httpClient,
            _configuration,
            NullLogger<OpenWeatherProvider>.Instance);

        var weatherForecast = await provider.GetWeatherForecastAsync(
            Location.Create("New York", "US"), numDays);

        var weatherForecastList = weatherForecast.ToList();

        Assert.NotNull(weatherForecastList);
        Assert.Equal(numDays, weatherForecastList.Count);
    }
}
