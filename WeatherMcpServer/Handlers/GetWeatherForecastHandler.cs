using MediatR;
using WeatherMcpServer.Commands;
using WeatherMcpServer.Services;
using Serilog;
using System.Text;

namespace WeatherMcpServer.Handlers;

/// <summary>
/// Handler for processing weather forecast requests
/// </summary>
public class GetWeatherForecastHandler(IWeatherService weatherService, ILogger logger) : IRequestHandler<GetWeatherForecastCommand, string>
{
    /// <summary>
    /// Handles the weather forecast request and returns formatted forecast information
    /// </summary>
    /// <param name="request">The weather forecast command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Formatted weather forecast information as string</returns>
    public async Task<string> Handle(GetWeatherForecastCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var locations = await weatherService.GetLocationAsync(request.City, request.CountryCode);
            
            if (locations.Length == 0)
            {
                return $"City '{request.City}' not found. Please check the spelling and try again.";
            }

            var location = locations[0];
            var weather = await weatherService.GetWeatherAsync(
                location.Latitude, 
                location.Longitude, 
                exclude: "minutely,hourly,current,alerts", 
                units: request.Units,
                language: request.Language);

            if (weather.Daily == null || weather.Daily.Length == 0)
            {
                return "Weather forecast data is not available for this location.";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"📅 **{request.Days}-Day Weather Forecast for {location.Name}, {location.Country}**");
            sb.AppendLine($"📍 Coordinates: {location.Latitude:F2}, {location.Longitude:F2}");
            sb.AppendLine();

            var daysToShow = Math.Min(request.Days, weather.Daily.Length);
            
            for (int i = 0; i < daysToShow; i++)
            {
                var day = weather.Daily[i];
                var date = DateTimeOffset.FromUnixTimeSeconds(day.DateTime).ToString("dddd, MMM dd");
                var condition = day.Weather.FirstOrDefault();
                
                sb.AppendLine($"**{date}**");
                sb.AppendLine($"🌤️  {condition?.Description ?? "Unknown"}");
                sb.AppendLine($"🌡️  High: {day.Temperature.Max:F1}°{GetTemperatureUnit(request.Units)} | Low: {day.Temperature.Min:F1}°{GetTemperatureUnit(request.Units)}");
                sb.AppendLine($"💧 Humidity: {day.Humidity}% | 🌧️  Rain: {day.ProbabilityOfPrecipitation * 100:F0}%");
                sb.AppendLine($"💨 Wind: {day.WindSpeed:F1} {GetSpeedUnit(request.Units)}");
                
                if (!string.IsNullOrEmpty(day.Summary))
                {
                    sb.AppendLine($"📝 {day.Summary}");
                }
                
                sb.AppendLine();
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error getting weather forecast for {City}: {Message}", request.City, ex.Message);
            return $"Error retrieving weather forecast for {request.City}: {ex.Message}";
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