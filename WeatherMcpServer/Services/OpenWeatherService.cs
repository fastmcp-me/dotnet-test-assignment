using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherMcpServer.Clients;
using WeatherMcpServer.Formatters;
using WeatherMcpServer.Model;
using WeatherMcpServer.Options;
using WeatherMcpServer.Presenters;

namespace WeatherMcpServer.Services;

public sealed class OpenWeatherService(
    OpenWeatherHttpClient client, 
    IOptions<OpenWeatherOptions> options, 
    ILogger<OpenWeatherService> logger)
{
    private readonly string _apiKey = options.Value.ApiKey;

    public async Task<string> GetCurrentWeatherDescription(
        string city, 
        string? countryCode = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City name must be provided.", nameof(city));

        var location = GetLocationQuery(city, countryCode);
        var url = $"/data/2.5/weather?q={location}&appid={_apiKey}&units=metric&lang=en";

        logger.LogDebug("Calling GetCurrentWeatherDescription for city {City}, country {Country}", city, countryCode);

        try
        {
            var response = await client.GetJsonAsync(url, ct);
            return response.RootElement.ToCurrentWeatherDescription();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "API call failed for GetCurrentWeather: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<string> Get5Day3HourStepForecast(
        string city,
        string? countryCode = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City name must be provided.", nameof(city));

        var location = GetLocationQuery(city, countryCode);
        var url = $"/data/2.5/forecast?q={location}&appid={_apiKey}&units=metric&lang=en";

        logger.LogDebug("Calling Get5Day3HourStepForecast for city {City}, country {Country}", city, countryCode);

        try
        {
            var response = await client.GetJsonAsync(url, ct);
            return response.RootElement.ToDailyForecastDescription(3);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "API call failed for Get5Day3HourStepForecast: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<string> GetWeatherAlertsDescription(
        string city, 
        string? countryCode = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City name must be provided.", nameof(city));

        var geo = await GetCoordinates(city, countryCode, ct);

        var url =$"/data/3.0/onecall?lat={geo.Latitude}&lon={geo.Longitude}&appid={_apiKey}&exclude=current,minutely,hourly,daily&units=metric&lang=en";

        logger.LogDebug("Calling GetWeatherAlerts for city {City}, country {Country}", city, countryCode);

        try
        {
            var response = await client.GetJsonAsync(url, ct);
            return response.RootElement.ToAlertsDescription();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "API call failed for GetWeatherAlerts: {Message}", ex.Message);
            throw;
        }
    }

    private async Task<GeoCoordinate> GetCoordinates(
        string city, 
        string? countryCode = null,
        CancellationToken ct = default)
    {
        var location = GetLocationQuery(city, countryCode);
        var url = $"/geo/1.0/direct?q={location}&limit=1&appid={_apiKey}";

        logger.LogDebug("Calling GetCoordinates for city {City}, country {Country}", city, countryCode);

        try
        {
            var response = await client.GetJsonAsync(url, ct);
            return response.RootElement.GetGeoCoordinate();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "API call failed for GetCoordinates: {Message}", ex.Message);
            throw;
        }
    }
    
    private static string GetLocationQuery(string city, string? countryCode) =>
        string.IsNullOrWhiteSpace(countryCode)
            ? city
            : $"{city},{countryCode}";
}
