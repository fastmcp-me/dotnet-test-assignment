using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WeatherMcpServer.Clients;
using WeatherMcpServer.Options;

namespace WeatherMcpServer.Services;

public sealed class OpenWeatherService(
    OpenWeatherHttpClient client, 
    IOptions<OpenWeatherOptions> options, 
    ILogger<OpenWeatherService> logger)
{
    private readonly OpenWeatherOptions _options = options.Value;

    public async Task<JsonDocument> GetCurrentWeather(string city, string? countryCode = null)
    {
        var location = GetLocationQuery(city, countryCode);
        var url = $"{_options.BaseUrl}/data/2.5/weather?q={location}&appid={_options.ApiKey}&units=metric&lang=en";

        logger.LogDebug("Calling GetCurrentWeather for city {City}, country {Country}", city, countryCode);

        return await client.GetAsync<JsonDocument>(url);
    }

    public async Task<JsonDocument> GetWeatherForecast(string city, string? countryCode = null)
    {
        var location = GetLocationQuery(city, countryCode);
        var url = $"{_options.BaseUrl}/data/2.5/forecast?q={location}&appid={_options.ApiKey}&units=metric&lang=en";

        logger.LogDebug("Calling GetWeatherForecast for city {City}, country {Country}", city, countryCode);

        return await client.GetAsync<JsonDocument>(url);
    }

    public async Task<JsonDocument> GetWeatherAlerts(string city, string? countryCode = null)
    {
        var geoLocation = await GetCoordinates(city, countryCode);
        var latitude = geoLocation.RootElement[0].GetProperty("lat").GetDouble();
        var longitude = geoLocation.RootElement[0].GetProperty("lon").GetDouble();

        var url =
            $"{_options.BaseUrl}/data/3.0/onecall?lat={latitude}&lon={longitude}&appid={_options.ApiKey}" +
            $"&exclude=current,minutely,hourly,daily&units=metric&lang=en";

        logger.LogDebug("Calling GetWeatherAlerts for city {City}, country {Country}", city, countryCode);

        return await client.GetAsync<JsonDocument>(url);
    }

    private async Task<JsonDocument> GetCoordinates(string city, string? countryCode = null)
    {
        var location = GetLocationQuery(city, countryCode);
        var url = $"{_options.BaseUrl}/geo/1.0/direct?q={location}&limit=1&appid={_options.ApiKey}";

        logger.LogDebug("Calling GetCoordinates for city {City}, country {Country}", city, countryCode);

        return await client.GetAsync<JsonDocument>(url);
    }
    
    private static string GetLocationQuery(string city, string? countryCode) =>
        string.IsNullOrWhiteSpace(countryCode)
            ? city
            : $"{city},{countryCode}";
}
