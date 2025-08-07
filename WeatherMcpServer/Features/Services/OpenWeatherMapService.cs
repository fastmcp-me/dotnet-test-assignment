using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherMcpServer.Configurations;
using WeatherMcpServer.Exceptions;
using WeatherMcpServer.Features.Services.DTOs.Interfaces;
using WeatherMcpServer.Features.Services.DTOs.OpenWeatherMap.CurrentWeather;
using WeatherMcpServer.Features.Services.DTOs.OpenWeatherMap.WeatherForecast;
using WeatherMcpServer.Features.Services.Interfaces;
using System.Text.Json;

namespace WeatherMcpServer.Features.Services;

/// <summary>
/// A service for getting weather information from OpenWeatherMap.
/// </summary>
internal class OpenWeatherMapService(
    HttpClient httpClient,
    IOptions<WeatherApiConfiguration> weatherApiConfiguration,
    ILogger<OpenWeatherMapService> logger) : IWeatherService
{
    private readonly static JsonSerializerOptions _jsonOptions;
    private readonly string _apiKey = weatherApiConfiguration.Value.ApiKey;
    private readonly string _units = weatherApiConfiguration.Value.Units;

    static OpenWeatherMapService()
    {
        _jsonOptions = new JsonSerializerOptions();
        _jsonOptions.Converters.Add(new CurrentWeatherJsonConverter());
        _jsonOptions.Converters.Add(new WeatherForecastJsonConverter());
    }

    public async Task<ICurrentWeatherDto> GetCurrentWeather(string city, string? countryCode, CancellationToken cancellationToken)
    {
        logger.LogInformation(weatherApiConfiguration.Value.ApiKey);
        logger.LogInformation(weatherApiConfiguration.Value.Url);
        logger.LogInformation(weatherApiConfiguration.Value.Units);
        logger.LogInformation("Getting current weather data in {City}, {CountryCode}.", city, countryCode);

        var loc = GetLocationQuery(city, countryCode);
        var queryUrl = $"?q={loc}&units={_units}&appid={_apiKey}";

        var response = await httpClient.GetAsync($"weather{queryUrl}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new McpServerException(@$"City not found. Please check the spelling or try another city.");
            }

            var error = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new McpServerException(error);
        }

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<CurrentWeatherDto>(responseString, _jsonOptions)!;
    }

    public async Task<IWeatherForecastDto> GetWeatherForecast(string city, string? countryCode, int days, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting weather forecast data in {City}, {CountryCode} for {Days} days.", city, countryCode, days);

        var loc = GetLocationQuery(city, countryCode);
        var queryUrl = $"?q={loc}&units={_units}&cnt={days * 8}&appid={_apiKey}";
        var response = await httpClient.GetAsync($"forecast{queryUrl}", cancellationToken)!;
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new McpServerException(@$"City not found. Please check the spelling or try another city.");
            }

            var error = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new McpServerException(error);
        }

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<WeatherForecastDto>(responseString, _jsonOptions)!;
    }

    public Task<IWeatherAlertDto> GetWeatherAlerts(string city, string? countryCode, CancellationToken cancellationToken)
    {
        throw new McpServerException("This method is only available in a paid subscription.");
    }

    private static string GetLocationQuery(string city, string? countryCode)
        => string.IsNullOrEmpty(countryCode)
            ? city
            : $"{city},{countryCode}";
}
