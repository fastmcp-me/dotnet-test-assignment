using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
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
    private readonly OpenWeatherOptions _options = options.Value;

    public async Task<string> GetCurrentWeatherDescription(string city, string? countryCode = null)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City name must be provided.", nameof(city));

        var location = GetLocationQuery(city, countryCode);
        var url = $"{_options.BaseUrl}/data/2.5/weather?q={location}&appid={_options.ApiKey}&units=metric&lang=en";

        logger.LogDebug("Calling GetCurrentWeather for city {City}, country {Country}, url: {Url}", city, countryCode, url);

        try
        {
            var response = await client.GetAsync<JsonDocument>(url);
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
        string? countryCode = null)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City name must be provided.", nameof(city));

        var location = GetLocationQuery(city, countryCode);
        var url = $"{_options.BaseUrl}/data/2.5/forecast?q={location}&appid={_options.ApiKey}&units=metric&lang=en";

        logger.LogDebug("Calling Get5Day3HourStepForecast for city {City}, country {Country}, url: {Url}", city, countryCode, url);

        try
        {
            var response = await client.GetAsync<JsonDocument>(url);
            return response.RootElement.ToDailyForecastDescription(3);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "API call failed for Get5Day3HourStepForecast: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<string> GetWeatherAlertsDescription(string city, string? countryCode = null)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City name must be provided.", nameof(city));

        var geo = await GetCoordinates(city, countryCode);

        var url =$"{_options.BaseUrl}/data/3.0/onecall?lat={geo.Latitude}&lon={geo.Longitude}&appid={_options.ApiKey}&exclude=current,minutely,hourly,daily&units=metric&lang=en";

        logger.LogDebug("Calling GetWeatherAlerts for city {City}, country {Country}, url: {Url}", city, countryCode, url);

        try
        {
            var response = await client.GetAsync<JsonDocument>(url);
            return response.RootElement.ToAlertsDescription();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "API call failed for GetWeatherAlerts: {Message}", ex.Message);
            throw;
        }
    }

    private async Task<GeoCoordinate> GetCoordinates(string city, string? countryCode = null)
    {
        var location = GetLocationQuery(city, countryCode);
        var url = $"{_options.BaseUrl}/geo/1.0/direct?q={location}&limit=1&appid={_options.ApiKey}";

        logger.LogDebug("Calling GetCoordinates for city {City}, country {Country}", city, countryCode);

        try
        {
            var response = await client.GetAsync<JsonDocument>(url);
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
