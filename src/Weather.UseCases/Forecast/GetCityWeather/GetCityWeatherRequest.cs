using MediatR;
using Weather.Core.Interfaces;

namespace Weather.UseCases.Forecast.GetCityWeather;

public record GetCityWeatherRequest(
    string City,
    string? CountryCode,
    string? StateCode
) : IUseCaseRequest<string>, IRequest<string>;
