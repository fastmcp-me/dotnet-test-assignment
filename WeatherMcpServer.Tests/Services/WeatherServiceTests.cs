using System.Net;
using System.Text.Json;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Serilog;
using WeatherMcpServer.Models;
using WeatherMcpServer.Services;

namespace WeatherMcpServer.Tests.Services;

/// <summary>
/// Unit tests for WeatherService
/// </summary>
public class WeatherServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger> _loggerMock;
    private readonly WeatherService _weatherService;

    public WeatherServiceTests()
    {
        // Set up environment variable for tests
        Environment.SetEnvironmentVariable("OPENWEATHERMAP_API_KEY", "test-api-key");
        
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.openweathermap.org/")
        };
        _loggerMock = new Mock<ILogger>();
        _loggerMock.Setup(x => x.ForContext<WeatherService>()).Returns(_loggerMock.Object);
        _weatherService = new WeatherService(_httpClient, _loggerMock.Object);
    }

    [Fact]
    public void Constructor_ThrowsException_WhenApiKeyNotSet()
    {
        // Arrange
        Environment.SetEnvironmentVariable("OPENWEATHERMAP_API_KEY", null);
        
        // Act & Assert
        var act = () => new WeatherService(_httpClient, _loggerMock.Object);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("OPENWEATHERMAP_API_KEY environment variable is required");
        
        // Clean up
        Environment.SetEnvironmentVariable("OPENWEATHERMAP_API_KEY", "test-api-key");
    }

    [Theory]
    [InlineData("London", null, 1)]
    [InlineData("Paris", "FR", 5)]
    [InlineData("New York", "US", 3)]
    public async Task GetLocationAsync_ReturnsLocations_WhenApiCallSucceeds(string city, string? countryCode, int limit)
    {
        // Arrange
        var expectedLocations = new[]
        {
            new GeolocationResponse("London", new Dictionary<string, string>(), 51.5074, -0.1278, "GB", "England"),
            new GeolocationResponse("Paris", new Dictionary<string, string>(), 48.8566, 2.3522, "FR", "Île-de-France")
        };
        
        var jsonResponse = JsonSerializer.Serialize(expectedLocations.Take(limit));
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _weatherService.GetLocationAsync(city, countryCode, limit);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountLessOrEqualTo(limit);
        
        // Verify the request was made correctly
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri!.ToString().Contains($"geo/1.0/direct") &&
                req.RequestUri.ToString().Contains($"appid=test-api-key")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetLocationAsync_ReturnsEmptyArray_WhenLocationNotFound()
    {
        // Arrange
        var jsonResponse = JsonSerializer.Serialize(Array.Empty<GeolocationResponse>());
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _weatherService.GetLocationAsync("NonExistentCity");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetLocationAsync_ThrowsException_WhenApiCallFails()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act & Assert
        var act = async () => await _weatherService.GetLocationAsync("London");
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Theory]
    [InlineData(51.5074, -0.1278, "", "metric", "en")]
    [InlineData(40.7128, -74.0060, "hourly,daily", "imperial", "es")]
    [InlineData(48.8566, 2.3522, "minutely", "kelvin", "fr")]
    public async Task GetWeatherAsync_ReturnsWeatherData_WhenApiCallSucceeds(
        double latitude, double longitude, string exclude, string units, string language)
    {
        // Arrange
        var expectedWeatherResponse = new WeatherResponse(
            latitude, longitude, "UTC", 0, 
            new CurrentWeather(1234567890, 1234567800, 1234567900, 15.5, 16.2, 1013, 65, 
                8.5, 3.2, 20, 10000, 5.5, 180, 7.2, 
                new[] { new WeatherCondition(800, "Clear", "clear sky", "01d") }),
            null, null, null, null);

        var jsonResponse = JsonSerializer.Serialize(expectedWeatherResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _weatherService.GetWeatherAsync(latitude, longitude, exclude, units, language);

        // Assert
        result.Should().NotBeNull();
        result.Latitude.Should().Be(latitude);
        result.Longitude.Should().Be(longitude);
        result.Current.Should().NotBeNull();
        result.Current!.Temperature.Should().Be(15.5);

        // Verify the request was made correctly
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri!.ToString().Contains($"data/3.0/onecall") &&
                req.RequestUri.ToString().Contains($"lat={latitude}") &&
                req.RequestUri.ToString().Contains($"lon={longitude}") &&
                req.RequestUri.ToString().Contains($"units={units}") &&
                req.RequestUri.ToString().Contains($"lang={language}") &&
                req.RequestUri.ToString().Contains($"appid=test-api-key")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetWeatherAsync_IncludesExcludeParameter_WhenProvided()
    {
        // Arrange
        var weatherResponse = new WeatherResponse(51.5, -0.1, "UTC", 0, null, null, null, null, null);
        var jsonResponse = JsonSerializer.Serialize(weatherResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act
        await _weatherService.GetWeatherAsync(51.5, -0.1, "hourly,daily");

        // Assert
        _httpMessageHandlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.RequestUri!.ToString().Contains("exclude=hourly,daily")),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetWeatherAsync_ThrowsException_WhenApiCallFails()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act & Assert
        var act = async () => await _weatherService.GetWeatherAsync(51.5, -0.1);
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GetWeatherAsync_ThrowsException_WhenResponseCannotBeDeserialized()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("invalid json")
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponse);

        // Act & Assert
        var act = async () => await _weatherService.GetWeatherAsync(51.5, -0.1);
        await act.Should().ThrowAsync<JsonException>();
    }
}