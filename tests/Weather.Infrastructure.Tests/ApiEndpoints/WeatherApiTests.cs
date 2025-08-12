using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Weather.Infrastructure.Tests.Fixtures;

namespace Weather.Infrastructure.Tests.ApiEndpoints;

public class WeatherApiTests(WeatherProxy devProxy) : IClassFixture<WeatherProxy>
{
    private WeatherApi CreateWeatherApi()
    {
        var appSettingsOptions = Options.Create(new WeatherConfiguration
        {
            ApiKey = "12345"
        });

        return new WeatherApi(
            appSettingsOptions,
            devProxy.HttpClient,
            Mock.Of<ILogger<WeatherApi>>()
        );
    }
    [Fact]
    public async Task GetCurrentWeatherByCityAsync_ReturnsWeatherDto()
    {
        var weatherApi = CreateWeatherApi();
        var city = "London";
        var results = await weatherApi.GetCurrentWeatherByCityAsync(city);

        Assert.NotNull(results);
        Assert.True(results.City.ToLower() == city.ToLower());
    }

    [Fact]
    public async Task GetWeatherForecastByCityAsync_ReturnsWeatherDto()
    {
        var weatherApi = CreateWeatherApi();
        var city = "London";
        var results = await weatherApi.GetWeatherForecastByCityAsync(city, new DateTime(2025, 8, 13));

        Assert.NotNull(results);
        Assert.True(results.City.ToLower() == city.ToLower());
    }
}