using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherMcpServer.Configurations;
using WeatherMcpServer.Exceptions;

namespace WeatherMcpServer.Tests.Configuration;

public class ConfigurationTests
{
    [Fact]
    public void WeatherApiConfiguration_Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var config = new WeatherApiConfiguration
        {
            ApiKey = "test-key-123",
            Url = "https://api.openweathermap.org/data/2.5/",
            Units = "metric"
        };

        // Act & Assert
        config.ApiKey.Should().Be("test-key-123");
        config.Url.Should().Be("https://api.openweathermap.org/data/2.5/");
        config.Units.Should().Be("metric");
    }

    [Theory]
    [InlineData("metric")]
    [InlineData("imperial")]
    [InlineData("kelvin")]
    public void WeatherApiConfiguration_DifferentUnits_Work(string units)
    {
        // Arrange & Act
        var config = new WeatherApiConfiguration
        {
            Units = units
        };

        // Assert
        config.Units.Should().Be(units);
    }

    [Fact]
    public void McpServerException_WithMessage_CreatesCorrectException()
    {
        // Arrange
        const string message = "Test exception message";

        // Act
        var exception = new McpServerException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.Should().BeOfType<McpServerException>();
    }

    [Fact]
    public void WeatherApiConfiguration_DefaultValues_AreEmpty()
    {
        // Arrange & Act
        var config = new WeatherApiConfiguration();

        // Assert
        config.ApiKey.Should().BeNullOrEmpty();
        config.Url.Should().BeNullOrEmpty();
        config.Units.Should().BeNullOrEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void WeatherApiConfiguration_EmptyValues_AreHandled(string value)
    {
        // Arrange & Act
        var config = new WeatherApiConfiguration
        {
            ApiKey = value,
            Url = value,
            Units = value
        };

        // Assert
        config.ApiKey.Should().Be(value);
        config.Url.Should().Be(value);
        config.Units.Should().Be(value);
    }

    [Fact]
    public void WeatherApiConfiguration_LongValues_AreHandled()
    {
        // Arrange
        var longApiKey = new string('a', 1000);
        var longUrl = "https://" + new string('b', 500) + ".com/api/";
        var longUnits = new string('c', 100);

        // Act
        var config = new WeatherApiConfiguration
        {
            ApiKey = longApiKey,
            Url = longUrl,
            Units = longUnits
        };

        // Assert
        config.ApiKey.Should().Be(longApiKey);
        config.Url.Should().Be(longUrl);
        config.Units.Should().Be(longUnits);
    }
}