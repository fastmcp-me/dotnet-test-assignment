using MediatR;
using WeatherMcpServer.Business.Models;

namespace WeatherMcpServer.Business.Queries.GetCurrentWeather;

public record GetCurrentWeatherQuery(string City, string? CountryCode = null) : IRequest<CurrentWeatherData>;