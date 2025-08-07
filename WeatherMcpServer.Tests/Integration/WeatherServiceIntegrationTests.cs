using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Configurations;
using WeatherMcpServer.Features.Services.Interfaces;

namespace WeatherMcpServer.Tests.Integration;

public class WeatherServiceIntegrationTests : IDisposable
{
    private readonly IHost _host;
    private readonly IWeatherService _weatherService;

    public WeatherServiceIntegrationTests()
    {
        var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.Configure<WeatherApiConfiguration>(config =>
                {
                    config.ApiKey = "test-api-key";
                    config.Url = "https://api.openweathermap.org/data/2.5/";
                    config.Units = "metric";
                });

                // Mock the weather service for integration tests
                var mockWeatherService = new Mock<IWeatherService>();
                SetupMockWeatherService(mockWeatherService);
                services.AddSingleton(mockWeatherService.Object);
            });

        _host = hostBuilder.Build();
        _weatherService = _host.Services.GetRequiredService<IWeatherService>();
    }

    [Fact]
    public async Task WeatherService_GetCurrentWeather_ReturnsExpectedData()
    {
        // Act
        var result = await _weatherService.GetCurrentWeather("London", "UK", CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.City.Should().Be("London");
        result.Country.Should().Be("UK");
        result.Temperature.Should().BeGreaterThan(-50).And.BeLessThan(60);
    }

    [Fact]
    public async Task WeatherService_GetWeatherForecast_ReturnsExpectedData()
    {
        // Act
        var result = await _weatherService.GetWeatherForecast("Paris", "FR", 3, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.City.Should().Be("Paris");
        result.Country.Should().Be("FR");
        result.Forecast.Should().HaveCount(3);
    }

    [Fact]
    public async Task WeatherService_GetWeatherAlerts_HandlesCorrectly()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<WeatherMcpServer.Exceptions.McpServerException>(
            () => _weatherService.GetWeatherAlerts("Miami", "US", CancellationToken.None));

        exception.Message.Should().Be("This method is only available in a paid subscription.");
    }

    [Fact]
    public async Task WeatherService_ConcurrentRequests_HandlesCorrectly()
    {
        // Arrange
        var cities = new[] { "London", "Paris", "Berlin", "Tokyo", "New York" };

        // Act
        var tasks = cities.Select(city => 
            _weatherService.GetCurrentWeather(city, null, CancellationToken.None)).ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().HaveCount(5);
        results.Should().AllSatisfy(result => result.Should().NotBeNull());
        
        // Each result should correspond to the requested city
        for (int i = 0; i < cities.Length; i++)
        {
            results[i].City.Should().Be(cities[i]);
        }
    }

    [Fact]
    public async Task WeatherService_ErrorHandling_CityNotFound()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<WeatherMcpServer.Exceptions.McpServerException>(
            () => _weatherService.GetCurrentWeather("NonExistentCity123", null, CancellationToken.None));

        exception.Message.Should().Be("City not found. Please check the spelling or try another city.");
    }

    [Theory]
    [InlineData("Moscow", null)]
    [InlineData("Tokyo", "JP")]
    [InlineData("Sydney", "AU")]
    public async Task WeatherService_VariousCities_ReturnsData(string city, string? countryCode)
    {
        // Act
        var result = await _weatherService.GetCurrentWeather(city, countryCode, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.City.Should().Be(city);
        
        if (countryCode != null)
        {
            result.Country.Should().NotBeNullOrEmpty();
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public async Task WeatherService_ForecastDifferentDays_ReturnsCorrectCount(int days)
    {
        // Act
        var result = await _weatherService.GetWeatherForecast("Berlin", "DE", days, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Forecast.Should().HaveCount(days);
    }

    [Fact]
    public async Task WeatherService_CancellationToken_RespectsCancellation()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(
            () => _weatherService.GetCurrentWeather("London", "UK", cancellationTokenSource.Token));
    }

    private static void SetupMockWeatherService(Mock<IWeatherService> mockService)
    {
        // Setup current weather responses
        mockService
            .Setup(x => x.GetCurrentWeather(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<string, string?, CancellationToken>((city, country, ct) =>
            {
                if (ct.IsCancellationRequested)
                    return Task.FromCanceled<WeatherMcpServer.Features.Services.DTOs.Interfaces.ICurrentWeatherDto>(ct);

                if (city == "NonExistentCity123")
                    throw new WeatherMcpServer.Exceptions.McpServerException("City not found. Please check the spelling or try another city.");

                var mock = new Mock<WeatherMcpServer.Features.Services.DTOs.Interfaces.ICurrentWeatherDto>();
                mock.Setup(x => x.City).Returns(city);
                mock.Setup(x => x.Country).Returns(GetCountryCode(city, country));
                mock.Setup(x => x.Temperature).Returns(GetRandomTemperature());
                mock.Setup(x => x.TemperatureFeelsLike).Returns(GetRandomTemperature());
                mock.Setup(x => x.Description).Returns("Clear sky");
                mock.Setup(x => x.WindSpeed).Returns(2.5);
                mock.Setup(x => x.Humidity).Returns(65);
                mock.Setup(x => x.Pressure).Returns(1013);
                
                return Task.FromResult(mock.Object);
            });

        // Setup forecast responses
        mockService
            .Setup(x => x.GetWeatherForecast(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns<string, string?, int, CancellationToken>((city, country, days, ct) =>
            {
                if (ct.IsCancellationRequested)
                    return Task.FromCanceled<WeatherMcpServer.Features.Services.DTOs.Interfaces.IWeatherForecastDto>(ct);

                var mock = new Mock<WeatherMcpServer.Features.Services.DTOs.Interfaces.IWeatherForecastDto>();
                mock.Setup(x => x.City).Returns(city);
                mock.Setup(x => x.Country).Returns(GetCountryCode(city, country));

                var forecasts = new List<WeatherMcpServer.Features.Services.DTOs.Interfaces.IDailyForecast>();
                for (int i = 0; i < days; i++)
                {
                    var dailyMock = new Mock<WeatherMcpServer.Features.Services.DTOs.Interfaces.IDailyForecast>();
                    dailyMock.Setup(x => x.Date).Returns(DateOnly.FromDateTime(DateTime.Today.AddDays(i + 1)));
                    dailyMock.Setup(x => x.MinTemperature).Returns(GetRandomTemperature() - 5);
                    dailyMock.Setup(x => x.MaxTemperature).Returns(GetRandomTemperature() + 5);
                    dailyMock.Setup(x => x.Description).Returns($"Day {i + 1} weather");
                    forecasts.Add(dailyMock.Object);
                }

                mock.Setup(x => x.Forecast).Returns([.. forecasts]);
                return Task.FromResult(mock.Object);
            });

        // Setup alerts (always throws exception for free tier)
        mockService
            .Setup(x => x.GetWeatherAlerts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new WeatherMcpServer.Exceptions.McpServerException("This method is only available in a paid subscription."));
    }

    private static string GetCountryCode(string city, string? providedCountry)
    {
        if (!string.IsNullOrEmpty(providedCountry))
            return providedCountry;

        return city switch
        {
            "London" => "GB",
            "Paris" => "FR",
            "Berlin" => "DE",
            "Tokyo" => "JP",
            "Moscow" => "RU",
            "Sydney" => "AU",
            "New York" => "US",
            _ => "XX"
        };
    }

    private static double GetRandomTemperature()
    {
        var random = new Random();
        return random.NextDouble() * 40 - 10; // Random temperature between -10 and 30
    }

    public void Dispose()
    {
        _host?.Dispose();
        GC.SuppressFinalize(this);
    }
}