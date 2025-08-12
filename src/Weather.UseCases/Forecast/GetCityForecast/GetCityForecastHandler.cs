using MediatR;
using Microsoft.Extensions.Logging;
using System;
using Weather.Core.Exceptions;
using Weather.Core.Interfaces;

namespace Weather.UseCases.Forecast.GetCityForecast;

public class GetCityForecastHandler(IWeatherForecast forecast, ILogger<GetCityForecastHandler> logger) : IRequestHandler<GetCityForecastRequest, string>
{
	public async Task<string> Handle(GetCityForecastRequest request, CancellationToken cancellationToken)
	{
        var (city, date, countryCode, stateCode) = request;
		try
        {
            var weather = await forecast.GetWeatherForecastByCityAsync(city, date, countryCode, stateCode, cancellationToken);

            return $"The weather forecast for {weather.City} on {date:yyyy-MM-dd} is {weather.WeatherCondition} with a temperature of {weather.Temperature}°C. " +
                $"Data retrieved at {weather.Timestamp:HH:mm:ss} UTC.";

        }
        catch (WeatherArgumentException ex)
        {
            logger.LogError(ex, "Error getting weather for {City}", city);

            return $"Error getting weather for {city}: {ex.Message}";
        }
	}
}
