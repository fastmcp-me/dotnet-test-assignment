using FluentValidation;
using MediatR;
using WeatherMcpServer.Features.Services.Interfaces;

namespace WeatherMcpServer.Features.Queries;

/// <summary>
/// Contains the query and handler for getting the weather forecast.
/// </summary>
public static class GetWeatherForecast
{
    /// <summary>
    /// Represents the query for getting the weather forecast.
    /// </summary>
    /// <param name="City">The city name.</param>
    /// <param name="CountryCode">The country code.</param>
    /// <param name="Days">The number of days to forecast.</param>
    public record Query(string City, string? CountryCode, int Days)
        : WeatherBase.Query(City, CountryCode);

    /// <summary>
    /// Handles the <see cref="Query"/> for getting the weather forecast.
    /// </summary>
    internal class Handler(IWeatherService weatherService) : IRequestHandler<Query, string>
    {
        public async Task<string> Handle(Query request, CancellationToken cancellationToken)
        {
            var forecastData = await weatherService.GetWeatherForecast(request.City, request.CountryCode, request.Days, cancellationToken);
            
            var forecastStrings = forecastData.Forecast
                .Select(item => $"""
                    📅 {item.Date:dddd, MMMM d}
                    🌡️ {item.MinTemperature:F0}° - {item.MaxTemperature:F0}°
                    ☁️ {item.Description}
                    """);

            return $"""
                {request.Days}-days weather forecast for {forecastData.City}, {forecastData.Country}:
                
                {string.Join("\n\n", forecastStrings)}
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
            RuleFor(x => x.Days)
                .NotNull()
                .InclusiveBetween(1, 5);
        }
    }
}
