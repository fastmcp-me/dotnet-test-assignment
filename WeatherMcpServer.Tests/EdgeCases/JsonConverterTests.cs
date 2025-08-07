using WeatherMcpServer.Features.Services.DTOs.Interfaces;
using WeatherMcpServer.Features.Services.DTOs.OpenWeatherMap.CurrentWeather;
using WeatherMcpServer.Features.Services.DTOs.OpenWeatherMap.WeatherForecast;
using System.Text.Json;

namespace WeatherMcpServer.Tests.EdgeCases;

public class WeatherDataModelTests
{
    [Fact]
    public void ICurrentWeatherDto_PropertiesCanBeSet()
    {
        // Arrange & Act
        var mock = new Mock<ICurrentWeatherDto>();
        mock.Setup(x => x.City).Returns("London");
        mock.Setup(x => x.Country).Returns("UK");
        mock.Setup(x => x.Temperature).Returns(20.5);
        mock.Setup(x => x.TemperatureFeelsLike).Returns(22.0);
        mock.Setup(x => x.Description).Returns("Sunny");
        mock.Setup(x => x.WindSpeed).Returns(3.5);
        mock.Setup(x => x.Humidity).Returns(65.0);
        mock.Setup(x => x.Pressure).Returns(1013.0);

        var weatherData = mock.Object;

        // Assert
        weatherData.City.Should().Be("London");
        weatherData.Country.Should().Be("UK");
        weatherData.Temperature.Should().Be(20.5);
        weatherData.TemperatureFeelsLike.Should().Be(22.0);
        weatherData.Description.Should().Be("Sunny");
        weatherData.WindSpeed.Should().Be(3.5);
        weatherData.Humidity.Should().Be(65.0);
        weatherData.Pressure.Should().Be(1013.0);
    }

    [Fact]
    public void IWeatherForecastDto_ForecastPropertiesWork()
    {
        // Arrange
        var dailyForecastMock = new Mock<IDailyForecast>();
        dailyForecastMock.Setup(x => x.Date).Returns(new DateOnly(2024, 1, 15));
        dailyForecastMock.Setup(x => x.MinTemperature).Returns(10.0);
        dailyForecastMock.Setup(x => x.MaxTemperature).Returns(18.0);
        dailyForecastMock.Setup(x => x.Description).Returns("Partly cloudy");

        var forecastMock = new Mock<IWeatherForecastDto>();
        forecastMock.Setup(x => x.City).Returns("Berlin");
        forecastMock.Setup(x => x.Country).Returns("DE");
        forecastMock.Setup(x => x.Forecast).Returns([dailyForecastMock.Object]);

        // Act
        var forecast = forecastMock.Object;

        // Assert
        forecast.City.Should().Be("Berlin");
        forecast.Country.Should().Be("DE");
        forecast.Forecast.Should().HaveCount(1);

        var firstDay = forecast.Forecast[0];
        firstDay.Date.Should().Be(new DateOnly(2024, 1, 15));
        firstDay.MinTemperature.Should().Be(10.0);
        firstDay.MaxTemperature.Should().Be(18.0);
        firstDay.Description.Should().Be("Partly cloudy");
    }

    [Fact]
    public void IDailyForecast_DateRangeValidation()
    {
        // Arrange
        var mock = new Mock<IDailyForecast>();
        var today = DateOnly.FromDateTime(DateTime.Today);
        var tomorrow = today.AddDays(1);
        var nextWeek = today.AddDays(7);

        // Act & Assert - Test various dates
        mock.Setup(x => x.Date).Returns(today);
        mock.Object.Date.Should().Be(today);

        mock.Setup(x => x.Date).Returns(tomorrow);
        mock.Object.Date.Should().Be(tomorrow);

        mock.Setup(x => x.Date).Returns(nextWeek);
        mock.Object.Date.Should().Be(nextWeek);
    }

    [Theory]
    [InlineData(-40.0, 50.0)] // Extreme cold to hot
    [InlineData(0.0, 0.0)]    // Freezing point
    [InlineData(15.5, 25.3)]  // Typical temperatures
    public void ICurrentWeatherDto_TemperatureRanges(double temp, double feelsLike)
    {
        // Arrange
        var mock = new Mock<ICurrentWeatherDto>();
        mock.Setup(x => x.Temperature).Returns(temp);
        mock.Setup(x => x.TemperatureFeelsLike).Returns(feelsLike);

        // Act
        var weather = mock.Object;

        // Assert
        weather.Temperature.Should().Be(temp);
        weather.TemperatureFeelsLike.Should().Be(feelsLike);
    }

    [Theory]
    [InlineData(0.0, 100.0)]    // Humidity range
    [InlineData(50.0, 1050.0)]  // Typical pressure range
    public void ICurrentWeatherDto_HumidityAndPressureRanges(double humidity, double pressure)
    {
        // Arrange
        var mock = new Mock<ICurrentWeatherDto>();
        mock.Setup(x => x.Humidity).Returns(humidity);
        mock.Setup(x => x.Pressure).Returns(pressure);

        // Act
        var weather = mock.Object;

        // Assert
        weather.Humidity.Should().Be(humidity);
        weather.Pressure.Should().Be(pressure);
    }

    [Fact]
    public void IWeatherAlertDto_AllPropertiesWork()
    {
        // Arrange
        var mock = new Mock<IWeatherAlertDto>();
        mock.Setup(x => x.Type).Returns("Tornado");
        mock.Setup(x => x.Severity).Returns("Extreme");
        mock.Setup(x => x.Certainty).Returns("Observed");
        mock.Setup(x => x.Description).Returns("Tornado warning in effect for your area");

        // Act
        var alert = mock.Object;

        // Assert
        alert.Type.Should().Be("Tornado");
        alert.Severity.Should().Be("Extreme");
        alert.Certainty.Should().Be("Observed");
        alert.Description.Should().Be("Tornado warning in effect for your area");
    }

    [Theory]
    [InlineData("Hurricane", "Severe", "Likely")]
    [InlineData("Flood", "Moderate", "Possible")]
    [InlineData("Thunderstorm", "Minor", "Unlikely")]
    public void IWeatherAlertDto_VariousAlertTypes(string type, string severity, string certainty)
    {
        // Arrange
        var mock = new Mock<IWeatherAlertDto>();
        mock.Setup(x => x.Type).Returns(type);
        mock.Setup(x => x.Severity).Returns(severity);
        mock.Setup(x => x.Certainty).Returns(certainty);

        // Act
        var alert = mock.Object;

        // Assert
        alert.Type.Should().Be(type);
        alert.Severity.Should().Be(severity);
        alert.Certainty.Should().Be(certainty);
    }

    [Fact]
    public void IWeatherForecastDto_EmptyForecastHandling()
    {
        // Arrange
        var mock = new Mock<IWeatherForecastDto>();
        mock.Setup(x => x.City).Returns("EmptyCity");
        mock.Setup(x => x.Country).Returns("EC");
        mock.Setup(x => x.Forecast).Returns([]);

        // Act
        var forecast = mock.Object;

        // Assert
        forecast.City.Should().Be("EmptyCity");
        forecast.Country.Should().Be("EC");
        forecast.Forecast.Should().BeEmpty();
    }

    [Fact]
    public void IWeatherForecastDto_MultipleForecasts()
    {
        // Arrange
        var forecasts = new List<Mock<IDailyForecast>>();
        for (int i = 0; i < 5; i++)
        {
            var dailyMock = new Mock<IDailyForecast>();
            dailyMock.Setup(x => x.Date).Returns(DateOnly.FromDateTime(DateTime.Today.AddDays(i + 1)));
            dailyMock.Setup(x => x.MinTemperature).Returns(10.0 + i);
            dailyMock.Setup(x => x.MaxTemperature).Returns(20.0 + i);
            dailyMock.Setup(x => x.Description).Returns($"Day {i + 1} weather");
            forecasts.Add(dailyMock);
        }

        var forecastMock = new Mock<IWeatherForecastDto>();
        forecastMock.Setup(x => x.City).Returns("TestCity");
        forecastMock.Setup(x => x.Country).Returns("TC");
        forecastMock.Setup(x => x.Forecast).Returns([.. forecasts.Select(f => f.Object)]);

        // Act
        var forecast = forecastMock.Object;

        // Assert
        forecast.Forecast.Should().HaveCount(5);

        for (int i = 0; i < 5; i++)
        {
            var day = forecast.Forecast[i];
            day.Date.Should().Be(DateOnly.FromDateTime(DateTime.Today.AddDays(i + 1)));
            day.MinTemperature.Should().Be(10.0 + i);
            day.MaxTemperature.Should().Be(20.0 + i);
            day.Description.Should().Be($"Day {i + 1} weather");
        }
    }

    [Fact]
    public void CurrentWeatherJsonConverter_ValidJson_DeserializesCorrectly()
    {
        // Arrange
        var json = """
            {
                "name": "London",
                "sys": { "country": "GB" },
                "main": {
                    "temp": 15.5,
                    "feels_like": 14.2,
                    "humidity": 72,
                    "pressure": 1013
                },
                "weather": [
                    {
                        "description": "partly cloudy"
                    }
                ],
                "wind": {
                    "speed": 3.2
                }
            }
            """;

        var options = new JsonSerializerOptions();
        options.Converters.Add(new CurrentWeatherJsonConverter());

        // Act
        var result = JsonSerializer.Deserialize<CurrentWeatherDto>(json, options)!;

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
    }

    [Fact]
    public void WeatherForecastJsonConverter_ValidJson_DeserializesCorrectly()
    {
        // Arrange
        var json = """
            {
                "city": {
                    "name": "Paris",
                    "country": "FR"
                },
                "list": [
                    {
                        "dt": 1640995200,
                        "main": {
                            "temp": 8.0
                        },
                        "weather": [
                            {
                                "description": "light rain"
                            }
                        ]
                    },
                    {
                        "dt": 1640996200,
                        "main": {
                            "temp": 12.0
                        },
                        "weather": [
                            {
                                "description": "light rain"
                            }
                        ]
                    }
                ]
            }
            """;

        var options = new JsonSerializerOptions();
        options.Converters.Add(new WeatherForecastJsonConverter());

        // Act
        var result = JsonSerializer.Deserialize<WeatherForecastDto>(json, options)!;

        // Assert
        result.Should().NotBeNull();
        result.City.Should().Be("Paris");
        result.Country.Should().Be("FR");
        result.Forecast.Should().HaveCount(1);
        result.Forecast[0].Date.Should().Be(new DateOnly(2022, 1, 1));
        result.Forecast[0].MinTemperature.Should().Be(8.0);
        result.Forecast[0].MaxTemperature.Should().Be(12.0);
        result.Forecast[0].Description.Should().Be("light rain");
    }

    [Fact]
    public void WeatherForecastJsonConverter_EmptyList_ReturnsEmptyForecast()
    {
        // Arrange
        var json = """
            {
                "city": {
                    "name": "EmptyCity",
                    "country": "EC"
                },
                "list": []
            }
            """;

        var options = new JsonSerializerOptions();
        options.Converters.Add(new WeatherForecastJsonConverter());

        // Act
        var result = JsonSerializer.Deserialize<WeatherForecastDto>(json, options)!;

        // Assert
        result.Should().NotBeNull();
        result.City.Should().Be("EmptyCity");
        result.Country.Should().Be("EC");
        result.Forecast.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    public void CurrentWeatherJsonConverter_InvalidJson_ThrowsJsonException(string invalidJson)
    {
        // Arrange
        var options = new JsonSerializerOptions();
        options.Converters.Add(new CurrentWeatherJsonConverter());

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<CurrentWeatherDto>(invalidJson, options));
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"invalid\": \"json\"}")]
    public void CurrentWeatherJsonConverter_InvalidJson_ThrowsKeyNotFoundException(string invalidJson)
    {
        // Arrange
        var options = new JsonSerializerOptions();
        options.Converters.Add(new CurrentWeatherJsonConverter());

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() =>
            JsonSerializer.Deserialize<CurrentWeatherDto>(invalidJson, options));
    }
}