using MediatR;
using Microsoft.Extensions.Logging;
using System;
using Weather.Core.Exceptions;
using Weather.Core.Interfaces;

namespace Weather.UseCases.Forecast.GetAlert;

public class GetAlertHandler(IWeatherForecast forecast, ILogger<GetAlertHandler> logger) : IRequestHandler<GetAlertRequest, string>
{
	public async Task<string> Handle(GetAlertRequest request, CancellationToken cancellationToken)
	{
        var (latitude, longitude, date) = request;
		try
        {
            var alert = await forecast.GetWeatherAlertAsync(latitude, longitude, date, cancellationToken);

            return $"The alert for coordinates ({latitude}, {longitude}) on {date:yyyy-MM-dd} is {alert.AlertLevel.ToString()} \"{alert.Event}\" with a temperature of {alert.Temperature}°C. ";
        }
        catch (WeatherArgumentException ex)
        {
            logger.LogError(ex, "Error getting alert for coordinates ({lat}, {lon})", latitude, longitude);

            return $"Error getting alert for coordinates ({latitude}, {longitude})";
        }
	}
}
