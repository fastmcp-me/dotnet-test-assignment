using MediatR;

namespace WeatherMcpServer.Business.Queries.GetCityWeather;

public record GetCityWeatherQuery(string City) : IRequest<GetCityWeatherResponse>;

public record GetCityWeatherResponse(string WeatherDescription);