using System.Text.Json;
using WeatherMcpServer.Formatters;

namespace WeatherMcpServer.Tests;

public class OpenWeatherFormatterTests
{
    [Fact]
    public void ToCurrentWeatherDescription_ValidJson_ReturnsFormattedString()
    {
        // Arrange
        string json = File.ReadAllText("Responses/OpenWeather/current-weather.json");
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Act
        var result = root.ToCurrentWeatherDescription();

        // Assert
        Assert.Contains("moderate rain", result);
        Assert.Contains("temperature: 20,58°C", result);
        Assert.Contains("pressure: 1014 hPa", result);
    }

    [Fact]
    public void ToCurrentWeatherDescription_InvalidJson_ReturnsError()
    {
        // Arrange
        string json = """ { "invalid": true } """;
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Act
        var result = root.ToCurrentWeatherDescription();

        // Assert
        Assert.Equal("Invalid current weather data format.", result);
    }

    [Fact]
    public void ToDailyForecastDescription_ValidJson_ReturnsFilteredForecasts()
    {
        // Arrange
        string json = File.ReadAllText("Responses/OpenWeather/forecast-5day-3hour.json");
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Act
        var result = root.ToDailyForecastDescription(hourStep: 3);

        var expected = """
            04-08-2025 21:00: light rain, temperature: 20,05°C, pressure: 1011 hPa.
            05-08-2025 18:00: scattered clouds, temperature: 20,41°C, pressure: 1009 hPa.
            06-08-2025 18:00: light rain, temperature: 20,98°C, pressure: 1014 hPa.
            07-08-2025 18:00: light rain, temperature: 17,04°C, pressure: 1017 hPa.
            08-08-2025 18:00: light rain, temperature: 17,54°C, pressure: 1017 hPa.
            09-08-2025 18:00: broken clouds, temperature: 18,77°C, pressure: 1017 hPa.
            """;

        // Assert
        Assert.Equal(expected.Replace("\r", ""), result);
    }

    [Fact]
    public void ToDailyForecastDescription_NoForecast_ReturnsNoForecastMessage()
    {
        // Arrange
        string json = """
                        {
              "cod": "200",
              "message": 0,
              "cnt": 40,
              "list": [
                
              ],
              "city": {
                "id": 524901,
                "name": "Moscow",
                "coord": {
                  "lat": 55.7522,
                  "lon": 37.6156
                },
                "country": "RU",
                "population": 1000000,
                "timezone": 10800,
                "sunrise": 1754271618,
                "sunset": 1754328656
              }
            }
            """;
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Act
        var result = root.ToDailyForecastDescription(hourStep: 3);

        // Assert
        Assert.Equal("No forecast data available.", result);
    }

    [Fact]
    public void ToAlertsDescription_ValidJson_ReturnsFormattedString()
    {
        // Arrange
        string json = File.ReadAllText("Responses/OpenWeather/weather-alerts.json");
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Act
        var result = root.ToAlertsDescription();

        // Assert
        Assert.Contains("Sender: NWS Philadelphia - Mount Holly (New Jersey, Delaware, Southeastern Pennsylvania)", result);
        Assert.Contains("Description: ...SMALL CRAFT ADVISORY", result);
    }

    [Fact]
    public void ToAlertsDescription_InvalidJson_ReturnsError()
    {
        // Arrange
        string json = """ { "invalid": true } """;
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Act
        var result = root.ToAlertsDescription();

        // Assert
        Assert.Equal("Invalid weather alerts data format.", result);
    }

    [Fact]
    public void ToAlertsDescription_NoAlerts_ReturnsNoAlertsMessage()
    {
        // Arrange
        string json = """
                        {
              "lat": 33.44,
              "lon": -94.04,
              "timezone": "America/Chicago",
              "timezone_offset": -18000,
              "alerts": [

              ]
            }
            """;
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Act
        var result = root.ToAlertsDescription();

        // Assert
        Assert.Equal("No weather alerts for this location.", result);
    }

}
