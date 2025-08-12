using MediatR;
using Weather.Core.Interfaces;

namespace Weather.UseCases.Forecast.GetAlert;

public record GetAlertRequest(
    double Latitude,
    double Longitude,
    DateTime Date
) :  IUseCaseRequest<string>, IRequest<string>;
