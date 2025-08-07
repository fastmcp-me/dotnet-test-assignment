using MediatR;
using WeatherMcpServer.Features.Services.Interfaces;

namespace WeatherMcpServer.Features.Queries;

/// <summary>
/// Contains the query and handler for getting weather alerts.
/// </summary>
public static class GetWeatherAlerts
{
    /// <summary>
    /// Represents the query for getting weather alerts.
    /// </summary>
    /// <param name="City">The city name.</param>
    /// <param name="CountryCode">The country code.</param>
    public record Query(string City, string? CountryCode)
        : WeatherBase.Query(City, CountryCode);

    /// <summary>
    /// Handles the <see cref="Query"/> for getting weather alerts.
    /// </summary>
    internal class Handler(IWeatherService weatherService) : IRequestHandler<Query, string>
    {
        public async Task<string> Handle(Query request, CancellationToken cancellationToken)
        {
            var alertsData = await weatherService.GetWeatherAlerts(request.City, request.CountryCode, cancellationToken);
            
            if (alertsData == null)
            {
                return $"No weather alerts for {request.City}.";
            }

            return $"""
                🚨 Weather alert for {request.City}:
                ⚠️ Type: {alertsData.Type}
                📊 Severity: {alertsData.Severity}
                🔍 Certainty: {alertsData.Certainty}
                📝 Description: {alertsData.Description}
                """;
        }
    }

    /// <summary>
    /// Validates the <see cref="Query"/>.
    /// </summary>
    internal class Validator : WeatherBase.Validator<Query>
    {
        public Validator()
        {
        }
    }
}
