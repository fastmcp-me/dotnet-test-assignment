using FluentValidation.TestHelper;
using WeatherMcpServer.Features.Queries;

namespace WeatherMcpServer.Tests.Queries;

public class WeatherValidationTests
{
    [Fact]
    public void GetCurrentWeatherValidator_ValidQuery_PassesValidation()
    {
        // Arrange
        var validator = new GetCurrentWeather.Validator();
        var query = new GetCurrentWeather.Query("London", "UK");

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetCurrentWeatherValidator_InvalidCity_FailsValidation(string? city)
    {
        // Arrange
        var validator = new GetCurrentWeather.Validator();
        var query = new GetCurrentWeather.Query(city!, "UK");

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    [Fact]
    public void GetCurrentWeatherValidator_NullCountryCode_PassesValidation()
    {
        // Arrange
        var validator = new GetCurrentWeather.Validator();
        var query = new GetCurrentWeather.Query("London", null);

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetWeatherForecastValidator_ValidQuery_PassesValidation()
    {
        // Arrange
        var validator = new GetWeatherForecast.Validator();
        var query = new GetWeatherForecast.Query("Paris", "FR", 3);

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetWeatherForecastValidator_InvalidCity_FailsValidation(string? city)
    {
        // Arrange
        var validator = new GetWeatherForecast.Validator();
        var query = new GetWeatherForecast.Query(city!, "FR", 3);

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(10)]
    public void GetWeatherForecastValidator_InvalidDays_FailsValidation(int days)
    {
        // Arrange
        var validator = new GetWeatherForecast.Validator();
        var query = new GetWeatherForecast.Query("Paris", "FR", days);

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Days);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void GetWeatherForecastValidator_ValidDays_PassesValidation(int days)
    {
        // Arrange
        var validator = new GetWeatherForecast.Validator();
        var query = new GetWeatherForecast.Query("Paris", "FR", days);

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Days);
    }

    [Fact]
    public void GetWeatherAlertsValidator_ValidQuery_PassesValidation()
    {
        // Arrange
        var validator = new GetWeatherAlerts.Validator();
        var query = new GetWeatherAlerts.Query("Miami", "US");

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetWeatherAlertsValidator_InvalidCity_FailsValidation(string? city)
    {
        // Arrange
        var validator = new GetWeatherAlerts.Validator();
        var query = new GetWeatherAlerts.Query(city!, "US");

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City);
    }

    [Fact]
    public void GetWeatherAlertsValidator_NullCountryCode_PassesValidation()
    {
        // Arrange
        var validator = new GetWeatherAlerts.Validator();
        var query = new GetWeatherAlerts.Query("Miami", null);

        // Act
        var result = validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}