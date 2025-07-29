using MediatR;
using WeatherMcpServer.Business.Models;

namespace WeatherMcpServer.Business.Queries.GetWeatherForecast;

public record GetWeatherForecastQuery(string City, string? CountryCode = null, int Days = 3) : IRequest<WeatherForecastData>;