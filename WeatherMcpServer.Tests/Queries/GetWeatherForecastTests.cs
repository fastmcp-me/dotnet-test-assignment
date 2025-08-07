using WeatherMcpServer.Features.Queries;
using WeatherMcpServer.Features.Services.DTOs.Interfaces;
using WeatherMcpServer.Features.Services.Interfaces;

namespace WeatherMcpServer.Tests.Queries;

public class GetWeatherForecastTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly GetWeatherForecast.Handler _handler;

    public GetWeatherForecastTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _handler = new GetWeatherForecast.Handler(_weatherServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsFormattedForecastString()
    {
        // Arrange
        var query = new GetWeatherForecast.Query("Paris", "FR", 3);
        var forecastData = CreateMockForecastData("Paris", "FR", 3);

        _weatherServiceMock
            .Setup(x => x.GetWeatherForecast("Paris", "FR", 3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecastData.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("3-days weather forecast for Paris, FR:");
        result.Should().Contain("📅 понедельник, января 1");
        result.Should().Contain("🌡️ 10° - 15°");
        result.Should().Contain("☁️ Sunny");
        result.Should().Contain("📅 вторник, января 2");
        result.Should().Contain("🌡️ 10° - 15°");
        result.Should().Contain("☁️ Cloudy");

        _weatherServiceMock.Verify(
            x => x.GetWeatherForecast("Paris", "FR", 3, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_SingleDay_ReturnsCorrectFormat()
    {
        // Arrange
        var query = new GetWeatherForecast.Query("Berlin", "DE", 1);
        var forecastData = CreateMockForecastData("Berlin", "DE", 1);

        _weatherServiceMock
            .Setup(x => x.GetWeatherForecast("Berlin", "DE", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecastData.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Contain("1-days weather forecast for Berlin, DE:");
        result.Should().Contain("📅 понедельник, января 1");
        result.Should().NotContain("Tuesday, January 2");
    }

    [Fact]
    public async Task Handle_MaximumDays_ReturnsCorrectFormat()
    {
        // Arrange
        var query = new GetWeatherForecast.Query("Tokyo", "JP", 5);
        var forecastData = CreateMockForecastData("Tokyo", "JP", 5);

        _weatherServiceMock
            .Setup(x => x.GetWeatherForecast("Tokyo", "JP", 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecastData.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Contain("5-days weather forecast for Tokyo, JP:");
        
        // Should contain all 5 days
        for (int i = 1; i <= 5; i++)
        {
            var date = new DateOnly(2024, 1, i);
            result.Should().Contain($"📅 {date:dddd, MMMM d}");
        }
    }

    [Fact]
    public async Task Handle_QueryWithoutCountryCode_CallsServiceCorrectly()
    {
        // Arrange
        var query = new GetWeatherForecast.Query("NewYork", null, 2);
        var forecastData = CreateMockForecastData("NewYork", "United States", 2);

        _weatherServiceMock
            .Setup(x => x.GetWeatherForecast("NewYork", null, 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecastData.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Contain("2-days weather forecast for NewYork, United States:");

        _weatherServiceMock.Verify(
            x => x.GetWeatherForecast("NewYork", null, 2, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(-5.0, 2.0)]
    [InlineData(0.0, 0.0)]
    [InlineData(25.5, 30.7)]
    public async Task Handle_DifferentTemperatures_FormatsCorrectly(double minTemp, double maxTemp)
    {
        // Arrange
        var query = new GetWeatherForecast.Query("TestCity", "TC", 1);
        var forecastData = CreateMockForecastData(minTemperature: minTemp, maxTemperature: maxTemp);

        _weatherServiceMock
            .Setup(x => x.GetWeatherForecast(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecastData.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Contain($"🌡️ {minTemp:F0}° - {maxTemp:F0}°");
    }

    [Fact]
    public async Task Handle_ServiceThrowsException_PropagatesException()
    {
        // Arrange
        var query = new GetWeatherForecast.Query("InvalidCity", null, 3);
        _weatherServiceMock
            .Setup(x => x.GetWeatherForecast(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Service error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

    private static Mock<IWeatherForecastDto> CreateMockForecastData(
        string city = "Berlin",
        string country = "Germany",
        int days = 2,
        double minTemperature = 10.0,
        double maxTemperature = 15.0)
    {
        var mock = new Mock<IWeatherForecastDto>();
        mock.Setup(x => x.City).Returns(city);
        mock.Setup(x => x.Country).Returns(country);

        var forecast = new List<Mock<IDailyForecast>>();
        for (int i = 0; i < days; i++)
        {
            var dailyMock = new Mock<IDailyForecast>();
            dailyMock.Setup(x => x.Date).Returns(new DateOnly(2024, 1, i + 1));
            dailyMock.Setup(x => x.MinTemperature).Returns(minTemperature);
            dailyMock.Setup(x => x.MaxTemperature).Returns(maxTemperature);
            dailyMock.Setup(x => x.Description).Returns(i == 0 ? "Sunny" : "Cloudy");
            forecast.Add(dailyMock);
        }

        mock.Setup(x => x.Forecast).Returns([.. forecast.Select(f => f.Object)]);
        return mock;
    }
}