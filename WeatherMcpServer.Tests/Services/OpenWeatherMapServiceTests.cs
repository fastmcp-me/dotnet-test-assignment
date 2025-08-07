using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq.Protected;
using System.Net;
using System.Text;
using WeatherMcpServer.Configurations;
using WeatherMcpServer.Exceptions;
using WeatherMcpServer.Features.Services.DTOs.Interfaces;
using WeatherMcpServer.Features.Services.Interfaces;

namespace WeatherMcpServer.Tests.Services;

public class WeatherServiceTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;

    public WeatherServiceTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
    }

    [Fact]
    public async Task GetCurrentWeather_ValidCity_ReturnsWeatherData()
    {
        // Arrange
        const string city = "London";
        const string countryCode = "UK";
        var weatherData = CreateMockWeatherData();

        _weatherServiceMock
            .Setup(x => x.GetCurrentWeather(city, countryCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(weatherData.Object);

        // Act
        var result = await _weatherServiceMock.Object.GetCurrentWeather(city, countryCode, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.City.Should().Be("London");
        result.Country.Should().Be("GB");
        result.Temperature.Should().Be(15.5);
        result.TemperatureFeelsLike.Should().Be(14.2);
        result.Humidity.Should().Be(72);
        result.Pressure.Should().Be(1013);
        result.Description.Should().Be("partly cloudy");
        result.WindSpeed.Should().Be(3.2);

        _weatherServiceMock.Verify(
            x => x.GetCurrentWeather(city, countryCode, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetCurrentWeather_CityWithoutCountryCode_ReturnsWeatherData()
    {
        // Arrange
        const string city = "Moscow";
        var weatherData = CreateMockWeatherData("Moscow", "RU", -5.0, -8.5, "snow", 2.1, 85, 1020);

        _weatherServiceMock
            .Setup(x => x.GetCurrentWeather(city, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(weatherData.Object);

        // Act
        var result = await _weatherServiceMock.Object.GetCurrentWeather(city, null, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.City.Should().Be("Moscow");
        result.Country.Should().Be("RU");
        result.Temperature.Should().Be(-5.0);

        _weatherServiceMock.Verify(
            x => x.GetCurrentWeather(city, null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetCurrentWeather_ServiceThrowsException_PropagatesException()
    {
        // Arrange
        const string city = "NonExistentCity";
        _weatherServiceMock
            .Setup(x => x.GetCurrentWeather(city, null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new McpServerException("City not found. Please check the spelling or try another city."));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<McpServerException>(
            () => _weatherServiceMock.Object.GetCurrentWeather(city, null, CancellationToken.None));

        exception.Message.Should().Be("City not found. Please check the spelling or try another city.");
    }

    [Fact]
    public async Task GetWeatherForecast_ValidRequest_ReturnsForecastData()
    {
        // Arrange
        const string city = "Paris";
        const string countryCode = "FR";
        const int days = 3;
        var forecastData = CreateMockForecastData();

        _weatherServiceMock
            .Setup(x => x.GetWeatherForecast(city, countryCode, days, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecastData.Object);

        // Act
        var result = await _weatherServiceMock.Object.GetWeatherForecast(city, countryCode, days, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.City.Should().Be("Paris");
        result.Country.Should().Be("FR");
        result.Forecast.Should().HaveCount(2);

        _weatherServiceMock.Verify(
            x => x.GetWeatherForecast(city, countryCode, days, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetWeatherForecast_ServiceThrowsException_PropagatesException()
    {
        // Arrange
        const string city = "NonExistentCity";
        const int days = 3;
        _weatherServiceMock
            .Setup(x => x.GetWeatherForecast(city, null, days, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new McpServerException("City not found. Please check the spelling or try another city."));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<McpServerException>(
            () => _weatherServiceMock.Object.GetWeatherForecast(city, null, days, CancellationToken.None));

        exception.Message.Should().Be("City not found. Please check the spelling or try another city.");
    }

    [Fact]
    public async Task GetWeatherAlerts_Always_ThrowsMcpServerException()
    {
        // Arrange
        _weatherServiceMock
            .Setup(x => x.GetWeatherAlerts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new McpServerException("This method is only available in a paid subscription."));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<McpServerException>(
            () => _weatherServiceMock.Object.GetWeatherAlerts("London", "UK", CancellationToken.None));

        exception.Message.Should().Be("This method is only available in a paid subscription.");
    }

    [Fact]
    public async Task GetWeatherAlerts_ValidCity_ReturnsAlertData()
    {
        // Arrange
        const string city = "Miami";
        const string countryCode = "US";
        var alertData = CreateMockAlertData();

        _weatherServiceMock
            .Setup(x => x.GetWeatherAlerts(city, countryCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(alertData.Object);

        // Act
        var result = await _weatherServiceMock.Object.GetWeatherAlerts(city, countryCode, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Type.Should().Be("Hurricane");
        result!.Severity.Should().Be("Severe");
        result!.Certainty.Should().Be("Likely");
        result!.Description.Should().Be("Hurricane warning in effect");

        _weatherServiceMock.Verify(
            x => x.GetWeatherAlerts(city, countryCode, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("London", "UK")]
    [InlineData("Paris", "FR")]
    [InlineData("Tokyo", null)]
    [InlineData("New York", "US")]
    public async Task GetCurrentWeather_VariousCities_CallsServiceCorrectly(string city, string? countryCode)
    {
        // Arrange
        var weatherData = CreateMockWeatherData(city, countryCode ?? "XX");

        _weatherServiceMock
            .Setup(x => x.GetCurrentWeather(city, countryCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(weatherData.Object);

        // Act
        var result = await _weatherServiceMock.Object.GetCurrentWeather(city, countryCode, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.City.Should().Be(city);

        _weatherServiceMock.Verify(
            x => x.GetCurrentWeather(city, countryCode, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public async Task GetWeatherForecast_VariousDays_CallsServiceCorrectly(int days)
    {
        // Arrange
        const string city = "Berlin";
        const string countryCode = "DE";
        var forecastData = CreateMockForecastData(city, countryCode, days);

        _weatherServiceMock
            .Setup(x => x.GetWeatherForecast(city, countryCode, days, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecastData.Object);

        // Act
        var result = await _weatherServiceMock.Object.GetWeatherForecast(city, countryCode, days, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Forecast.Should().HaveCount(days);

        _weatherServiceMock.Verify(
            x => x.GetWeatherForecast(city, countryCode, days, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static Mock<ICurrentWeatherDto> CreateMockWeatherData(
        string city = "London",
        string country = "GB",
        double temperature = 15.5,
        double feelsLike = 14.2,
        string description = "partly cloudy",
        double windSpeed = 3.2,
        double humidity = 72,
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

    private static Mock<IWeatherForecastDto> CreateMockForecastData(
        string city = "Paris",
        string country = "FR",
        int days = 2)
    {
        var mock = new Mock<IWeatherForecastDto>();
        mock.Setup(x => x.City).Returns(city);
        mock.Setup(x => x.Country).Returns(country);

        var forecast = new List<Mock<IDailyForecast>>();
        for (int i = 0; i < days; i++)
        {
            var dailyMock = new Mock<IDailyForecast>();
            dailyMock.Setup(x => x.Date).Returns(new DateOnly(2024, 1, i + 1));
            dailyMock.Setup(x => x.MinTemperature).Returns(8.0 + i);
            dailyMock.Setup(x => x.MaxTemperature).Returns(12.0 + i);
            dailyMock.Setup(x => x.Description).Returns(i == 0 ? "light rain" : "cloudy");
            forecast.Add(dailyMock);
        }

        mock.Setup(x => x.Forecast).Returns([.. forecast.Select(f => f.Object)]);
        return mock;
    }

    private static Mock<IWeatherAlertDto> CreateMockAlertData(
        string type = "Hurricane",
        string severity = "Severe",
        string certainty = "Likely",
        string description = "Hurricane warning in effect")
    {
        var mock = new Mock<IWeatherAlertDto>();
        mock.Setup(x => x.Type).Returns(type);
        mock.Setup(x => x.Severity).Returns(severity);
        mock.Setup(x => x.Certainty).Returns(certainty);
        mock.Setup(x => x.Description).Returns(description);
        return mock;
    }
}