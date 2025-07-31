using MediatR;
using WeatherMcpServer.Commands;
using WeatherMcpServer.Services;
using Serilog;
using System.Text;

namespace WeatherMcpServer.Handlers;

/// <summary>
/// Handler for processing weather alerts requests
/// </summary>
public class GetWeatherAlertsHandler(IWeatherService weatherService, ILogger logger) : IRequestHandler<GetWeatherAlertsCommand, string>
{
    /// <summary>
    /// Handles the weather alerts request and returns formatted alerts information
    /// </summary>
    /// <param name="request">The weather alerts command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Formatted weather alerts information as string</returns>
    public async Task<string> Handle(GetWeatherAlertsCommand request, CancellationToken cancellationToken)
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
                exclude: "minutely,hourly,daily,current", 
                language: request.Language);

            var sb = new StringBuilder();
            sb.AppendLine($"🚨 **Weather Alerts for {location.Name}, {location.Country}**");
            sb.AppendLine($"📍 Coordinates: {location.Latitude:F2}, {location.Longitude:F2}");
            sb.AppendLine();

            if (weather.Alerts == null || weather.Alerts.Length == 0)
            {
                sb.AppendLine("✅ No active weather alerts for this location.");
                return sb.ToString();
            }

            for (int i = 0; i < weather.Alerts.Length; i++)
            {
                var alert = weather.Alerts[i];
                var start = DateTimeOffset.FromUnixTimeSeconds(alert.Start);
                var end = DateTimeOffset.FromUnixTimeSeconds(alert.End);
                
                sb.AppendLine($"**Alert {i + 1}: {alert.Event}**");
                sb.AppendLine($"🏢 Issued by: {alert.SenderName}");
                sb.AppendLine($"⏰ From: {start:MMM dd, HH:mm} to {end:MMM dd, HH:mm}");
                sb.AppendLine($"📝 {alert.Description}");
                
                if (alert.Tags?.Length > 0)
                {
                    sb.AppendLine($"🏷️  Tags: {string.Join(", ", alert.Tags)}");
                }
                
                sb.AppendLine();
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error getting weather alerts for {City}: {Message}", request.City, ex.Message);
            return $"Error retrieving weather alerts for {request.City}: {ex.Message}";
        }
    }
}