using MediatR;
using Weather.Core.Interfaces;

namespace Weather.UseCases.Forecast.GetCityForecast;

public record GetCityForecastRequest(
    string City,
    DateTime Date,
    string? CountryCode,
    string? StateCode
) : IUseCaseRequest<string>, IRequest<string>;
