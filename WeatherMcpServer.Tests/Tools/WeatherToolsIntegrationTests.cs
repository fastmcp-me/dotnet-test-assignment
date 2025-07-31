using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Serilog;
using WeatherMcpServer.Commands;
using WeatherMcpServer.Handlers;
using WeatherMcpServer.Services;
using WeatherMcpServer.Tools;

namespace WeatherMcpServer.Tests.Tools;

/// <summary>
/// Integration tests for WeatherTools
/// </summary>
public class WeatherToolsIntegrationTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly Mock<ILogger> _loggerMock;

    public WeatherToolsIntegrationTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _loggerMock = new Mock<ILogger>();
        _loggerMock.Setup(x => x.ForContext<WeatherTools>()).Returns(_loggerMock.Object);
        _loggerMock.Setup(x => x.ForContext<GetCurrentWeatherHandler>()).Returns(_loggerMock.Object);
        _loggerMock.Setup(x => x.ForContext<GetWeatherForecastHandler>()).Returns(_loggerMock.Object);
        _loggerMock.Setup(x => x.ForContext<GetWeatherAlertsHandler>()).Returns(_loggerMock.Object);

        var services = new ServiceCollection();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetCurrentWeatherHandler).Assembly));
        services.AddScoped(_ => _weatherServiceMock.Object);
        services.AddScoped(_ => _loggerMock.Object);
        
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task GetCurrentWeather_ReturnsWeatherData_WhenValidCityProvided()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var weatherTools = new WeatherTools(mediator, _loggerMock.Object);

        var locations = new[]
        {
            new WeatherMcpServer.Models.GeolocationResponse("London", new Dictionary<string, string>(), 51.5074, -0.1278, "GB", "England")
        };

        var weatherResponse = new WeatherMcpServer.Models.WeatherResponse(
            51.5074, -0.1278, "Europe/London", 0,
            new WeatherMcpServer.Models.CurrentWeather(1234567890, 1234567800, 1234567900, 15.5, 16.2, 1013, 65,
                8.5, 3.2, 20, 10000, 5.5, 180, 7.2,
                new[] { new WeatherMcpServer.Models.WeatherCondition(800, "Clear", "clear sky", "01d") }),
            null, null, null, null);

        _weatherServiceMock.Setup(x => x.GetLocationAsync("London", "UK", 1))
            .ReturnsAsync(locations);

        _weatherServiceMock.Setup(x => x.GetWeatherAsync(51.5074, -0.1278, "minutely,hourly,daily,alerts", "metric", "en"))
            .ReturnsAsync(weatherResponse);

        // Act
        var result = await weatherTools.GetCurrentWeather("London", "UK", "metric", "en");

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("London, GB");
        result.Should().Contain("15,5°C");
        result.Should().Contain("clear sky");
        result.Should().Contain("65%"); // humidity
        
        // Verify the service was called with correct parameters
        _weatherServiceMock.Verify(x => x.GetLocationAsync("London", "UK", 1), Times.Once);
        _weatherServiceMock.Verify(x => x.GetWeatherAsync(51.5074, -0.1278, "minutely,hourly,daily,alerts", "metric", "en"), Times.Once);
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsFormattedForecast_WhenValidCityProvided()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var weatherTools = new WeatherTools(mediator, _loggerMock.Object);

        var locations = new[]
        {
            new WeatherMcpServer.Models.GeolocationResponse("Paris", new Dictionary<string, string>(), 48.8566, 2.3522, "FR", "Île-de-France")
        };

        var dailyWeather = new[]
        {
            new WeatherMcpServer.Models.DailyWeather(1234567890, 1234567800, 1234567900, 1234567850, 1234567950, 0.5, "Sunny day",
                new WeatherMcpServer.Models.DailyTemperature(20.5, 15.2, 25.8, 18.3, 22.1, 16.7),
                new WeatherMcpServer.Models.DailyFeelsLike(21.2, 19.1, 23.4, 17.8),
                1015, 60, 12.3, 3.2, 180, 5.1,
                new[] { new WeatherMcpServer.Models.WeatherCondition(800, "Clear", "clear sky", "01d") },
                30, 0.0, null, null, 4.5),
            new WeatherMcpServer.Models.DailyWeather(1234654290, 1234654200, 1234654300, 1234654250, 1234654350, 0.7, "Cloudy day",
                new WeatherMcpServer.Models.DailyTemperature(18.2, 12.5, 23.1, 16.8, 19.9, 14.3),
                new WeatherMcpServer.Models.DailyFeelsLike(19.1, 17.2, 20.8, 15.9),
                1008, 75, 8.9, 4.8, 220, 6.7,
                new[] { new WeatherMcpServer.Models.WeatherCondition(801, "Clouds", "few clouds", "02d") },
                80, 0.2, null, null, 2.1)
        };

        var weatherResponse = new WeatherMcpServer.Models.WeatherResponse(
            48.8566, 2.3522, "Europe/Paris", 3600,
            null, null, null, dailyWeather, null);

        _weatherServiceMock.Setup(x => x.GetLocationAsync("Paris", "FR", 1))
            .ReturnsAsync(locations);

        _weatherServiceMock.Setup(x => x.GetWeatherAsync(48.8566, 2.3522, "minutely,hourly,current,alerts", "metric", "en"))
            .ReturnsAsync(weatherResponse);

        // Act
        var result = await weatherTools.GetWeatherForecast("Paris", "FR", 2, "metric", "en");

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("Paris, FR");
        result.Should().Contain("2-Day Weather Forecast");
        result.Should().Contain("clear sky");
        result.Should().Contain("few clouds");
        result.Should().Contain("High: 25,8°C");
        result.Should().Contain("Low: 15,2°C");
        result.Should().Contain("Sunny day");
        result.Should().Contain("Cloudy day");
        
        // Verify the service was called with correct parameters
        _weatherServiceMock.Verify(x => x.GetLocationAsync("Paris", "FR", 1), Times.Once);
        _weatherServiceMock.Verify(x => x.GetWeatherAsync(48.8566, 2.3522, "minutely,hourly,current,alerts", "metric", "en"), Times.Once);
    }

    [Fact]
    public async Task GetWeatherAlerts_ReturnsAlertsInformation_WhenValidCityProvided()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var weatherTools = new WeatherTools(mediator, _loggerMock.Object);

        var locations = new[]
        {
            new WeatherMcpServer.Models.GeolocationResponse("Miami", new Dictionary<string, string>(), 25.7617, -80.1918, "US", "Florida")
        };

        var alerts = new[]
        {
            new WeatherMcpServer.Models.WeatherAlert("National Weather Service", "Thunderstorm Warning", 1234567890, 1234654290,
                "Severe thunderstorm warning in effect for Miami-Dade County until 8 PM.",
                new[] { "Thunderstorm", "Wind", "Hail" })
        };

        var weatherResponse = new WeatherMcpServer.Models.WeatherResponse(
            25.7617, -80.1918, "America/New_York", -18000,
            null, null, null, null, alerts);

        _weatherServiceMock.Setup(x => x.GetLocationAsync("Miami", "US", 1))
            .ReturnsAsync(locations);

        _weatherServiceMock.Setup(x => x.GetWeatherAsync(25.7617, -80.1918, "minutely,hourly,daily,current", "metric", "en"))
            .ReturnsAsync(weatherResponse);

        // Act
        var result = await weatherTools.GetWeatherAlerts("Miami", "US", "en");

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("Miami, US");
        result.Should().Contain("Thunderstorm Warning");
        result.Should().Contain("National Weather Service");
        result.Should().Contain("Severe thunderstorm warning");
        result.Should().Contain("Thunderstorm, Wind, Hail");
        
        // Verify the service was called with correct parameters
        _weatherServiceMock.Verify(x => x.GetLocationAsync("Miami", "US", 1), Times.Once);
        _weatherServiceMock.Verify(x => x.GetWeatherAsync(25.7617, -80.1918, "minutely,hourly,daily,current", "metric", "en"), Times.Once);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(5, 5)]
    [InlineData(8, 8)]
    [InlineData(10, 8)] // Should be clamped to 8
    [InlineData(0, 1)]  // Should be clamped to 1
    public async Task GetWeatherForecast_ClampsDaysParameter_ToValidRange(int requestedDays, int expectedDays)
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var weatherTools = new WeatherTools(mediator, _loggerMock.Object);

        var locations = new[]
        {
            new WeatherMcpServer.Models.GeolocationResponse("London", new Dictionary<string, string>(), 51.5074, -0.1278, "GB", "England")
        };

        // Create enough daily weather data for 8 days
        var dailyWeather = Enumerable.Range(0, 8).Select(i =>
            new WeatherMcpServer.Models.DailyWeather(1234567890 + i * 86400, 1234567800 + i * 86400, 1234567900 + i * 86400, 
                1234567850 + i * 86400, 1234567950 + i * 86400, 0.5, $"Day {i + 1}",
                new WeatherMcpServer.Models.DailyTemperature(20.5, 15.2, 25.8, 18.3, 22.1, 16.7),
                new WeatherMcpServer.Models.DailyFeelsLike(21.2, 19.1, 23.4, 17.8),
                1015, 60, 12.3, 3.2, 180, 5.1,
                new[] { new WeatherMcpServer.Models.WeatherCondition(800, "Clear", "clear sky", "01d") },
                30, 0.0, null, null, 4.5)
        ).ToArray();

        var weatherResponse = new WeatherMcpServer.Models.WeatherResponse(
            51.5074, -0.1278, "Europe/London", 0,
            null, null, null, dailyWeather, null);

        _weatherServiceMock.Setup(x => x.GetLocationAsync("London", null, 1))
            .ReturnsAsync(locations);

        _weatherServiceMock.Setup(x => x.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(weatherResponse);

        // Act
        var result = await weatherTools.GetWeatherForecast("London", null, requestedDays);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain($"{expectedDays}-Day Weather Forecast");
    }

    [Fact]
    public async Task GetCurrentWeather_HandlesInvalidCity_Gracefully()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var weatherTools = new WeatherTools(mediator, _loggerMock.Object);

        _weatherServiceMock.Setup(x => x.GetLocationAsync("InvalidCity", null, 1))
            .ReturnsAsync(Array.Empty<WeatherMcpServer.Models.GeolocationResponse>());

        // Act
        var result = await weatherTools.GetCurrentWeather("InvalidCity");

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("City 'InvalidCity' not found");
        
        // Verify the service was called
        _weatherServiceMock.Verify(x => x.GetLocationAsync("InvalidCity", null, 1), Times.Once);
        _weatherServiceMock.Verify(x => x.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetWeatherAlerts_ShowsNoAlertsMessage_WhenNoAlertsExist()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        var weatherTools = new WeatherTools(mediator, _loggerMock.Object);

        var locations = new[]
        {
            new WeatherMcpServer.Models.GeolocationResponse("London", new Dictionary<string, string>(), 51.5074, -0.1278, "GB", "England")
        };

        var weatherResponse = new WeatherMcpServer.Models.WeatherResponse(
            51.5074, -0.1278, "Europe/London", 0,
            null, null, null, null, null); // No alerts

        _weatherServiceMock.Setup(x => x.GetLocationAsync("London", null, 1))
            .ReturnsAsync(locations);

        _weatherServiceMock.Setup(x => x.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(weatherResponse);

        // Act
        var result = await weatherTools.GetWeatherAlerts("London");

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("No active weather alerts");
    }

}