using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net.Http;
using WeatherMcpServer.Application;
using WeatherMcpServer.Domain.LocationAggregate;
using WeatherMcpServer.Infrastructure.OpenWeather;
using Xunit;

namespace IntegrationTests.Application;

public class WeatherServiceTests : TestBase
{

    [Fact]
    public async Task GetLocationWeatherAsync_ShoulReturnValidLocationWeatherEntity_ForKnownCityAndDays()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var provider = new OpenWeatherProvider(
            _httpClient,
            _configuration,
            NullLogger<OpenWeatherProvider>.Instance);

        var weatherService = new WeatherService(
            provider,
            _cacheService,
            NullLogger<WeatherService>.Instance);

        var location = Location.Create("London", "GB");
        var numDays = 3;

        var locationWeather = await weatherService.GetLocationWeatherAsync(location, numDays);

        Assert.NotNull(locationWeather);
    }
}
