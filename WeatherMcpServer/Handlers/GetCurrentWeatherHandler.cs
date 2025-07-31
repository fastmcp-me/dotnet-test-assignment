using MediatR;
using WeatherMcpServer.Commands;
using WeatherMcpServer.Services;
using Serilog;
using System.Diagnostics;
using System.Text;

namespace WeatherMcpServer.Handlers;

/// <summary>
/// Handler for processing current weather requests
/// </summary>
public class GetCurrentWeatherHandler(IWeatherService weatherService, ILogger logger) : IRequestHandler<GetCurrentWeatherCommand, string>
{
    /// <summary>
    /// Handles the current weather request and returns formatted weather information
    /// </summary>
    /// <param name="request">The current weather command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Formatted current weather information as string</returns>
    public async Task<string> Handle(GetCurrentWeatherCommand request, CancellationToken cancellationToken)
    {
        using var activity = new Activity("HandleCurrentWeather").Start();
        activity?.SetTag("weather.command", "GetCurrentWeather");
        activity?.SetTag("weather.city", request.City);
        activity?.SetTag("weather.country_code", request.CountryCode);
        
        var stopwatch = Stopwatch.StartNew();
        
        logger.Information("Processing current weather request for {City}, {CountryCode} with units: {Units}", 
            request.City, request.CountryCode, request.Units);
        
        try
        {
            var locations = await weatherService.GetLocationAsync(request.City, request.CountryCode);
            
            if (locations.Length == 0)
            {
                logger.Warning("No locations found for city: {City}, country: {CountryCode}", 
                    request.City, request.CountryCode);
                return $"City '{request.City}' not found. Please check the spelling and try again.";
            }

            var location = locations[0];
            logger.Debug("Using location: {LocationName}, {Country} ({Latitude}, {Longitude})", 
                location.Name, location.Country, location.Latitude, location.Longitude);
            
            var weather = await weatherService.GetWeatherAsync(
                location.Latitude, 
                location.Longitude, 
                exclude: "minutely,hourly,daily,alerts", 
                units: request.Units,
                language: request.Language);

            if (weather.Current == null)
            {
                logger.Warning("Current weather data is null for location: {LocationName}, {Country}", 
                    location.Name, location.Country);
                return "Current weather data is not available for this location.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"🌍 **Current Weather in {location.Name}, {location.Country}**");
            sb.AppendLine($"📍 Coordinates: {location.Latitude:F2}, {location.Longitude:F2}");
            sb.AppendLine();
            
            var current = weather.Current;
            var condition = current.Weather.FirstOrDefault();
            
            sb.AppendLine($"🌤️  **Condition:** {condition?.Description ?? "Unknown"}");
            sb.AppendLine($"🌡️  **Temperature:** {current.Temperature:F1}°{GetTemperatureUnit(request.Units)}");
            sb.AppendLine($"🤔 **Feels like:** {current.FeelsLike:F1}°{GetTemperatureUnit(request.Units)}");
            sb.AppendLine($"💨 **Wind:** {current.WindSpeed:F1} {GetSpeedUnit(request.Units)} at {current.WindDegree}°");
            sb.AppendLine($"💧 **Humidity:** {current.Humidity}%");
            sb.AppendLine($"📊 **Pressure:** {current.Pressure} hPa");
            sb.AppendLine($"☁️  **Clouds:** {current.Clouds}%");
            sb.AppendLine($"👁️  **Visibility:** {current.Visibility / 1000.0:F1} km");
            
            if (current.UvIndex > 0)
            {
                sb.AppendLine($"☀️  **UV Index:** {current.UvIndex:F1}");
            }

            var result = sb.ToString();
            
            logger.Information("Successfully generated current weather report for {City} in {ElapsedMs}ms", 
                request.City, stopwatch.ElapsedMilliseconds);
            logger.Debug("Current weather report length: {ReportLength} characters", result.Length);
            
            activity?.SetTag("weather.success", true);
            activity?.SetTag("weather.duration_ms", stopwatch.ElapsedMilliseconds);
            
            return result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error getting current weather for {City}: {Message}", request.City, ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            return $"Error retrieving weather data for {request.City}: {ex.Message}";
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    /// <summary>
    /// Gets the temperature unit symbol based on the units parameter
    /// </summary>
    /// <param name="units">The units parameter (metric, imperial, kelvin)</param>
    /// <returns>Temperature unit symbol (C, F, or K)</returns>
    private static string GetTemperatureUnit(string units) => units switch
    {
        "imperial" => "F",
        "kelvin" => "K",
        _ => "C"
    };

    /// <summary>
    /// Gets the wind speed unit based on the units parameter
    /// </summary>
    /// <param name="units">The units parameter (metric, imperial, kelvin)</param>
    /// <returns>Speed unit (m/s or mph)</returns>
    private static string GetSpeedUnit(string units) => units switch
    {
        "imperial" => "mph",
        _ => "m/s"
    };
}