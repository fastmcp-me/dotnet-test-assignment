using WeatherMcpServer.Features.Queries;
using WeatherMcpServer.Features.Services.DTOs.Interfaces;
using WeatherMcpServer.Features.Services.Interfaces;

namespace WeatherMcpServer.Tests.Queries;

public class GetCurrentWeatherTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly GetCurrentWeather.Handler _handler;

    public GetCurrentWeatherTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _handler = new GetCurrentWeather.Handler(_weatherServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsFormattedWeatherString()
    {
        // Arrange
        var query = new GetCurrentWeather.Query("London", "UK");
        var weatherData = CreateMockWeatherData();

        _weatherServiceMock
            .Setup(x => x.GetCurrentWeather("London", "UK", It.IsAny<CancellationToken>()))
            .ReturnsAsync(weatherData.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("Current weather in London, United Kingdom:");
        result.Should().Contain("Temperature: 22,5° (feels like 25,0°)");
        result.Should().Contain("Conditions: Partly cloudy");
        result.Should().Contain("Wind: 3,2 m/s");
        result.Should().Contain("Humidity: 65%");
        result.Should().Contain("Pressure: 1013 hPa");

        _weatherServiceMock.Verify(
            x => x.GetCurrentWeather("London", "UK", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_QueryWithoutCountryCode_ReturnsFormattedWeatherString()
    {
        // Arrange
        var query = new GetCurrentWeather.Query("Moscow", null);
        var weatherData = CreateMockWeatherData("Moscow", "Russia");

        _weatherServiceMock
            .Setup(x => x.GetCurrentWeather("Moscow", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(weatherData.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("Current weather in Moscow, Russia:");

        _weatherServiceMock.Verify(
            x => x.GetCurrentWeather("Moscow", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(-10.5, -12.0)]
    [InlineData(0.0, 0.0)]
    [InlineData(35.7, 40.2)]
    public async Task Handle_DifferentTemperatures_FormatsCorrectly(double temperature, double feelsLike)
    {
        // Arrange
        var query = new GetCurrentWeather.Query("TestCity", "TC");
        var weatherData = CreateMockWeatherData(temperature: temperature, feelsLike: feelsLike);

        _weatherServiceMock
            .Setup(x => x.GetCurrentWeather(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(weatherData.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Contain($"Temperature: {temperature:F1}° (feels like {feelsLike:F1}°)");
    }

    [Fact]
    public async Task Handle_ServiceThrowsException_PropagatesException()
    {
        // Arrange
        var query = new GetCurrentWeather.Query("InvalidCity", null);
        _weatherServiceMock
            .Setup(x => x.GetCurrentWeather(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Service error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CancellationRequested_PropagatesCancellation()
    {
        // Arrange
        var query = new GetCurrentWeather.Query("London", "UK");
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        _weatherServiceMock
            .Setup(x => x.GetCurrentWeather(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(query, cancellationTokenSource.Token));
    }

    private static Mock<ICurrentWeatherDto> CreateMockWeatherData(
        string city = "London",
        string country = "United Kingdom",
        double temperature = 22.5,
        double feelsLike = 25.0,
        string description = "Partly cloudy",
        double windSpeed = 3.2,
        double humidity = 65,
        double pressure = 1013)
    {
        var mock = new Mock<ICurrentWeatherDto>();
        mock.Setup(x => x.City).Returns(city);
        mock.Setup(x => x.Country).Returns(country);
        mock.Setup(x => x.Temperature).Returns(temperature);
        mock.Setup(x => x.TemperatureFeelsLike).Returns(feelsLike);
        mock.Setup(x => x.Description).Returns(description);
        mock.Setup(x => x.WindSpeed).Returns(windSpeed);
        mock.Setup(x => x.Humidity).Returns(humidity);
        mock.Setup(x => x.Pressure).Returns(pressure);
        return mock;
    }
}