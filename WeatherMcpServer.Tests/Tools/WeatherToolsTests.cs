using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherMcpServer.Configurations;
using WeatherMcpServer.Features.Queries;
using WeatherMcpServer.Tools;

namespace WeatherMcpServer.Tests.Tools;

public class WeatherToolsTests
{
    private readonly Mock<ILogger<WeatherTools>> _loggerMock;
    private readonly Mock<IOptions<WeatherApiConfiguration>> _optionsMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly WeatherTools _weatherTools;
    private readonly WeatherApiConfiguration _config;

    public WeatherToolsTests()
    {
        _loggerMock = new Mock<ILogger<WeatherTools>>();
        _optionsMock = new Mock<IOptions<WeatherApiConfiguration>>();
        _mediatorMock = new Mock<IMediator>();

        _config = new WeatherApiConfiguration
        {
            ApiKey = "test-api-key-12345",
            Url = "https://api.openweathermap.org/data/2.5/",
            Units = "metric"
        };

        _optionsMock.Setup(x => x.Value).Returns(_config);

        _weatherTools = new WeatherTools(_loggerMock.Object, _optionsMock.Object, _mediatorMock.Object);
    }

    [Fact]
    public async Task GetCurrentWeather_ValidParameters_CallsMediatorWithCorrectQuery()
    {
        // Arrange
        const string city = "London";
        const string countryCode = "UK";
        const string expectedResponse = "Weather data for London";

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetCurrentWeather.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _weatherTools.GetCurrentWeather(city, countryCode, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);

        _mediatorMock.Verify(
            x => x.Send(
                It.Is<GetCurrentWeather.Query>(q => q.City == city && q.CountryCode == countryCode),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetCurrentWeather_NullCountryCode_CallsMediatorCorrectly()
    {
        // Arrange
        const string city = "Paris";
        const string expectedResponse = "Weather data for Paris";

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetCurrentWeather.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _weatherTools.GetCurrentWeather(city, null, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);

        _mediatorMock.Verify(
            x => x.Send(
                It.Is<GetCurrentWeather.Query>(q => q.City == city && q.CountryCode == null),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetWeatherForecast_ValidParameters_CallsMediatorWithCorrectQuery()
    {
        // Arrange
        const string city = "Tokyo";
        const string countryCode = "JP";
        const int days = 5;
        const string expectedResponse = "Forecast data for Tokyo";

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetWeatherForecast.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _weatherTools.GetWeatherForecast(city, countryCode, days, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);

        _mediatorMock.Verify(
            x => x.Send(
                It.Is<GetWeatherForecast.Query>(q => 
                    q.City == city && 
                    q.CountryCode == countryCode && 
                    q.Days == days),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetWeatherForecast_DefaultDays_UsesCorrectDefault()
    {
        // Arrange
        const string city = "Berlin";
        const string expectedResponse = "3-day forecast for Berlin";

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetWeatherForecast.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act - using default days parameter
        var result = await _weatherTools.GetWeatherForecast(city, "DE", cancellationToken: CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);

        _mediatorMock.Verify(
            x => x.Send(
                It.Is<GetWeatherForecast.Query>(q => 
                    q.City == city && 
                    q.CountryCode == "DE" && 
                    q.Days == 3), // Default value
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetWeatherAlerts_ValidParameters_CallsMediatorWithCorrectQuery()
    {
        // Arrange
        const string city = "Miami";
        const string countryCode = "US";
        const string expectedResponse = "Weather alerts for Miami";

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetWeatherAlerts.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _weatherTools.GetWeatherAlerts(city, countryCode, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);

        _mediatorMock.Verify(
            x => x.Send(
                It.Is<GetWeatherAlerts.Query>(q => q.City == city && q.CountryCode == countryCode),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetWeatherAlerts_NullCountryCode_CallsMediatorCorrectly()
    {
        // Arrange
        const string city = "Sydney";
        const string expectedResponse = "No alerts for Sydney";

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetWeatherAlerts.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _weatherTools.GetWeatherAlerts(city, null, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);

        _mediatorMock.Verify(
            x => x.Send(
                It.Is<GetWeatherAlerts.Query>(q => q.City == city && q.CountryCode == null),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AllMethods_CancellationToken_PropagatedToMediator()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<IRequest<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Test response");

        // Act & Assert - Test all three methods
        await _weatherTools.GetCurrentWeather("Test", "TC", cancellationToken);
        await _weatherTools.GetWeatherForecast("Test", "TC", 3, cancellationToken);
        await _weatherTools.GetWeatherAlerts("Test", "TC", cancellationToken);

        // Verify cancellation token was passed correctly
        _mediatorMock.Verify(
            x => x.Send(It.IsAny<IRequest<string>>(), cancellationToken),
            Times.Exactly(3));
    }

    [Fact]
    public async Task GetCurrentWeather_MediatorThrowsException_PropagatesException()
    {
        // Arrange
        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetCurrentWeather.Query>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Mediator error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _weatherTools.GetCurrentWeather("ErrorCity", null, CancellationToken.None));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("City-With-Special-Characters!@#")]
    [InlineData("Город")]
    public async Task GetCurrentWeather_VariousCityNames_PassedCorrectlyToMediator(string cityName)
    {
        // Arrange
        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetCurrentWeather.Query>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Test response");

        // Act
        await _weatherTools.GetCurrentWeather(cityName, "TC", CancellationToken.None);

        // Assert
        _mediatorMock.Verify(
            x => x.Send(
                It.Is<GetCurrentWeather.Query>(q => q.City == cityName),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}