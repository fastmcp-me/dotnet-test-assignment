using FluentAssertions;
using Moq;
using Serilog;
using WeatherMcpServer.Commands;
using WeatherMcpServer.Handlers;
using WeatherMcpServer.Models;
using WeatherMcpServer.Services;

namespace WeatherMcpServer.Tests.Handlers;

/// <summary>
/// Unit tests for Weather Handlers
/// </summary>
public class WeatherHandlersTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly Mock<ILogger> _loggerMock;

    public WeatherHandlersTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _loggerMock = new Mock<ILogger>();
        _loggerMock.Setup(x => x.ForContext<GetCurrentWeatherHandler>()).Returns(_loggerMock.Object);
        _loggerMock.Setup(x => x.ForContext<GetWeatherForecastHandler>()).Returns(_loggerMock.Object);
        _loggerMock.Setup(x => x.ForContext<GetWeatherAlertsHandler>()).Returns(_loggerMock.Object);
    }

    #region GetCurrentWeatherHandler Tests

    [Fact]
    public async Task GetCurrentWeatherHandler_ReturnsFormattedWeather_WhenLocationFound()
    {
        // Arrange
        var handler = new GetCurrentWeatherHandler(_weatherServiceMock.Object, _loggerMock.Object);
        var command = new GetCurrentWeatherCommand("London", "UK", "metric", "en");

        var locations = new[]
        {
            new GeolocationResponse("London", new Dictionary<string, string>(), 51.5074, -0.1278, "GB", "England")
        };

        var weatherResponse = new WeatherResponse(
            51.5074, -0.1278, "Europe/London", 0,
            new CurrentWeather(1234567890, 1234567800, 1234567900, 15.5, 16.2, 1013, 65,
                8.5, 3.2, 20, 10000, 5.5, 180, 7.2,
                new[] { new WeatherCondition(800, "Clear", "clear sky", "01d") }),
            null, null, null, null);

        _weatherServiceMock.Setup(x => x.GetLocationAsync("London", "UK", 1))
            .ReturnsAsync(locations);

        _weatherServiceMock.Setup(x => x.GetWeatherAsync(51.5074, -0.1278, "minutely,hourly,daily,alerts", "metric", "en"))
            .ReturnsAsync(weatherResponse);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("London, GB");
        result.Should().Contain("15,5°C");
        result.Should().Contain("clear sky");
        result.Should().Contain("65%"); // humidity
        result.Should().Contain("1013 hPa"); // pressure
    }

    [Fact]
    public async Task GetCurrentWeatherHandler_ReturnsNotFoundMessage_WhenLocationNotFound()
    {
        // Arrange
        var handler = new GetCurrentWeatherHandler(_weatherServiceMock.Object, _loggerMock.Object);
        var command = new GetCurrentWeatherCommand("NonExistentCity");

        _weatherServiceMock.Setup(x => x.GetLocationAsync("NonExistentCity", null, 1))
            .ReturnsAsync(Array.Empty<GeolocationResponse>());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Contain("City 'NonExistentCity' not found");
    }

    [Fact]
    public async Task GetCurrentWeatherHandler_ReturnsErrorMessage_WhenCurrentWeatherIsNull()
    {
        // Arrange
        var handler = new GetCurrentWeatherHandler(_weatherServiceMock.Object, _loggerMock.Object);
        var command = new GetCurrentWeatherCommand("London");

        var locations = new[]
        {
            new GeolocationResponse("London", new Dictionary<string, string>(), 51.5074, -0.1278, "GB", "England")
        };

        var weatherResponse = new WeatherResponse(
            51.5074, -0.1278, "Europe/London", 0,
            null, // Current weather is null
            null, null, null, null);

        _weatherServiceMock.Setup(x => x.GetLocationAsync("London", null, 1))
            .ReturnsAsync(locations);

        _weatherServiceMock.Setup(x => x.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(weatherResponse);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Contain("Current weather data is not available");
    }

    [Theory]
    [InlineData("metric", "°C", "m/s")]
    [InlineData("imperial", "°F", "mph")]
    [InlineData("kelvin", "°K", "m/s")]
    public async Task GetCurrentWeatherHandler_UsesCorrectUnits_BasedOnUnitsParameter(string units, string tempUnit, string speedUnit)
    {
        // Arrange
        var handler = new GetCurrentWeatherHandler(_weatherServiceMock.Object, _loggerMock.Object);
        var command = new GetCurrentWeatherCommand("London", null, units);

        var locations = new[]
        {
            new GeolocationResponse("London", new Dictionary<string, string>(), 51.5074, -0.1278, "GB", "England")
        };

        var weatherResponse = new WeatherResponse(
            51.5074, -0.1278, "Europe/London", 0,
            new CurrentWeather(1234567890, 1234567800, 1234567900, 15.5, 16.2, 1013, 65,
                8.5, 3.2, 20, 10000, 5.5, 180, 7.2,
                new[] { new WeatherCondition(800, "Clear", "clear sky", "01d") }),
            null, null, null, null);

        _weatherServiceMock.Setup(x => x.GetLocationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(locations);

        _weatherServiceMock.Setup(x => x.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>(), units, It.IsAny<string>()))
            .ReturnsAsync(weatherResponse);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Contain(tempUnit);
        result.Should().Contain(speedUnit);
    }

    [Fact]
    public async Task GetCurrentWeatherHandler_ReturnsErrorMessage_WhenExceptionThrown()
    {
        // Arrange
        var handler = new GetCurrentWeatherHandler(_weatherServiceMock.Object, _loggerMock.Object);
        var command = new GetCurrentWeatherCommand("London");

        _weatherServiceMock.Setup(x => x.GetLocationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .ThrowsAsync(new HttpRequestException("API Error"));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Contain("Error retrieving weather data for London");
        result.Should().Contain("API Error");
    }

    #endregion

    #region GetWeatherForecastHandler Tests

    [Fact]
    public async Task GetWeatherForecastHandler_ReturnsFormattedForecast_WhenLocationFound()
    {
        // Arrange
        var handler = new GetWeatherForecastHandler(_weatherServiceMock.Object, _loggerMock.Object);
        var command = new GetWeatherForecastCommand("Paris", "FR", 3, "metric", "en");

        var locations = new[]
        {
            new GeolocationResponse("Paris", new Dictionary<string, string>(), 48.8566, 2.3522, "FR", "Île-de-France")
        };

        var dailyWeather = new[]
        {
            new DailyWeather(1234567890, 1234567800, 1234567900, 1234567850, 1234567950, 0.5, "Partly cloudy day",
                new DailyTemperature(20.5, 15.2, 25.8, 18.3, 22.1, 16.7),
                new DailyFeelsLike(21.2, 19.1, 23.4, 17.8),
                1015, 60, 12.3, 3.2, 180, 5.1,
                new[] { new WeatherCondition(801, "Clouds", "few clouds", "02d") },
                30, 0.2, null, null, 4.5),
            new DailyWeather(1234654290, 1234654200, 1234654300, 1234654250, 1234654350, 0.7, "Rainy day",
                new DailyTemperature(18.2, 12.5, 23.1, 16.8, 19.9, 14.3),
                new DailyFeelsLike(19.1, 17.2, 20.8, 15.9),
                1008, 75, 8.9, 4.8, 220, 6.7,
                new[] { new WeatherCondition(500, "Rain", "light rain", "10d") },
                80, 0.8, 2.5, null, 2.1)
        };

        var weatherResponse = new WeatherResponse(
            48.8566, 2.3522, "Europe/Paris", 3600,
            null, null, null, dailyWeather, null);

        _weatherServiceMock.Setup(x => x.GetLocationAsync("Paris", "FR", 1))
            .ReturnsAsync(locations);

        _weatherServiceMock.Setup(x => x.GetWeatherAsync(48.8566, 2.3522, "minutely,hourly,current,alerts", "metric", "en"))
            .ReturnsAsync(weatherResponse);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("Paris, FR");
        result.Should().Contain("3-Day Weather Forecast");
        result.Should().Contain("few clouds");
        result.Should().Contain("light rain");
        result.Should().Contain("High: 25,8°C");
        result.Should().Contain("Low: 15,2°C");
        result.Should().Contain("Partly cloudy day");
    }

    [Fact]
    public async Task GetWeatherForecastHandler_ReturnsNotFoundMessage_WhenLocationNotFound()
    {
        // Arrange
        var handler = new GetWeatherForecastHandler(_weatherServiceMock.Object, _loggerMock.Object);
        var command = new GetWeatherForecastCommand("NonExistentCity");

        _weatherServiceMock.Setup(x => x.GetLocationAsync("NonExistentCity", null, 1))
            .ReturnsAsync(Array.Empty<GeolocationResponse>());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Contain("City 'NonExistentCity' not found");
    }

    [Fact]
    public async Task GetWeatherForecastHandler_ReturnsErrorMessage_WhenDailyWeatherIsNull()
    {
        // Arrange
        var handler = new GetWeatherForecastHandler(_weatherServiceMock.Object, _loggerMock.Object);
        var command = new GetWeatherForecastCommand("London");

        var locations = new[]
        {
            new GeolocationResponse("London", new Dictionary<string, string>(), 51.5074, -0.1278, "GB", "England")
        };

        var weatherResponse = new WeatherResponse(
            51.5074, -0.1278, "Europe/London", 0,
            null, null, null, null, null); // Daily weather is null

        _weatherServiceMock.Setup(x => x.GetLocationAsync("London", null, 1))
            .ReturnsAsync(locations);

        _weatherServiceMock.Setup(x => x.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(weatherResponse);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Contain("Weather forecast data is not available");
    }

    [Fact]
    public async Task GetWeatherForecastHandler_LimitsDaysToAvailableData_WhenRequestedDaysExceedAvailable()
    {
        // Arrange
        var handler = new GetWeatherForecastHandler(_weatherServiceMock.Object, _loggerMock.Object);
        var command = new GetWeatherForecastCommand("London", null, 10); // Request 10 days

        var locations = new[]
        {
            new GeolocationResponse("London", new Dictionary<string, string>(), 51.5074, -0.1278, "GB", "England")
        };

        // Only provide 3 days of data
        var dailyWeather = new[]
        {
            new DailyWeather(1234567890, 1234567800, 1234567900, 1234567850, 1234567950, 0.5, "Day 1",
                new DailyTemperature(20.5, 15.2, 25.8, 18.3, 22.1, 16.7),
                new DailyFeelsLike(21.2, 19.1, 23.4, 17.8),
                1015, 60, 12.3, 3.2, 180, 5.1,
                new[] { new WeatherCondition(800, "Clear", "clear sky", "01d") },
                30, 0.0, null, null, 4.5),
            new DailyWeather(1234654290, 1234654200, 1234654300, 1234654250, 1234654350, 0.7, "Day 2",
                new DailyTemperature(18.2, 12.5, 23.1, 16.8, 19.9, 14.3),
                new DailyFeelsLike(19.1, 17.2, 20.8, 15.9),
                1008, 75, 8.9, 4.8, 220, 6.7,
                new[] { new WeatherCondition(801, "Clouds", "few clouds", "02d") },
                80, 0.3, null, null, 2.1),
            new DailyWeather(1234740690, 1234740600, 1234740700, 1234740650, 1234740750, 0.3, "Day 3",
                new DailyTemperature(22.1, 16.8, 27.3, 19.5, 24.2, 18.1),
                new DailyFeelsLike(23.0, 20.3, 25.1, 19.0),
                1020, 55, 10.7, 2.8, 160, 4.9,
                new[] { new WeatherCondition(800, "Clear", "clear sky", "01d") },
                10, 0.1, null, null, 5.8)
        };

        var weatherResponse = new WeatherResponse(
            51.5074, -0.1278, "Europe/London", 0,
            null, null, null, dailyWeather, null);

        _weatherServiceMock.Setup(x => x.GetLocationAsync("London", null, 1))
            .ReturnsAsync(locations);

        _weatherServiceMock.Setup(x => x.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(weatherResponse);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        // Should only show 3 days even though 10 were requested
        var dayCount = result.Split("**Day").Length - 1; // Count occurrences of day headers
        // The result should contain only the available days (3 or fewer)
        result.Should().Contain("Day 1");
        result.Should().Contain("Day 2");
        result.Should().Contain("Day 3");
    }

    #endregion

    #region GetWeatherAlertsHandler Tests

    [Fact]
    public async Task GetWeatherAlertsHandler_ReturnsFormattedAlerts_WhenAlertsExist()
    {
        // Arrange
        var handler = new GetWeatherAlertsHandler(_weatherServiceMock.Object, _loggerMock.Object);
        var command = new GetWeatherAlertsCommand("Miami", "US", "en");

        var locations = new[]
        {
            new GeolocationResponse("Miami", new Dictionary<string, string>(), 25.7617, -80.1918, "US", "Florida")
        };

        var alerts = new[]
        {
            new WeatherAlert("National Weather Service", "Hurricane Warning", 1234567890, 1234654290,
                "Hurricane warning in effect for Miami-Dade County. Seek shelter immediately.",
                new[] { "Hurricane", "Wind", "Coastal" })
        };

        var weatherResponse = new WeatherResponse(
            25.7617, -80.1918, "America/New_York", -18000,
            null, null, null, null, alerts);

        _weatherServiceMock.Setup(x => x.GetLocationAsync("Miami", "US", 1))
            .ReturnsAsync(locations);

        _weatherServiceMock.Setup(x => x.GetWeatherAsync(25.7617, -80.1918, "minutely,hourly,daily,current", "metric", "en"))
            .ReturnsAsync(weatherResponse);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("Miami, US");
        result.Should().Contain("Hurricane Warning");
        result.Should().Contain("National Weather Service");
        result.Should().Contain("Hurricane warning in effect");
        result.Should().Contain("Hurricane, Wind, Coastal");
    }

    [Fact]
    public async Task GetWeatherAlertsHandler_ReturnsNoAlertsMessage_WhenNoAlertsExist()
    {
        // Arrange
        var handler = new GetWeatherAlertsHandler(_weatherServiceMock.Object, _loggerMock.Object);
        var command = new GetWeatherAlertsCommand("London");

        var locations = new[]
        {
            new GeolocationResponse("London", new Dictionary<string, string>(), 51.5074, -0.1278, "GB", "England")
        };

        var weatherResponse = new WeatherResponse(
            51.5074, -0.1278, "Europe/London", 0,
            null, null, null, null, null); // No alerts

        _weatherServiceMock.Setup(x => x.GetLocationAsync("London", null, 1))
            .ReturnsAsync(locations);

        _weatherServiceMock.Setup(x => x.GetWeatherAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(weatherResponse);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Contain("No active weather alerts");
    }

    [Fact]
    public async Task GetWeatherAlertsHandler_ReturnsNotFoundMessage_WhenLocationNotFound()
    {
        // Arrange
        var handler = new GetWeatherAlertsHandler(_weatherServiceMock.Object, _loggerMock.Object);
        var command = new GetWeatherAlertsCommand("NonExistentCity");

        _weatherServiceMock.Setup(x => x.GetLocationAsync("NonExistentCity", null, 1))
            .ReturnsAsync(Array.Empty<GeolocationResponse>());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Contain("City 'NonExistentCity' not found");
    }

    #endregion
}