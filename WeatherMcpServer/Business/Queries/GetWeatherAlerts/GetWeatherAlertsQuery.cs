using MediatR;
using WeatherMcpServer.Business.Models;

namespace WeatherMcpServer.Business.Queries.GetWeatherAlerts;

public record GetWeatherAlertsQuery(string City, string? CountryCode = null) : IRequest<WeatherAlertsData>;