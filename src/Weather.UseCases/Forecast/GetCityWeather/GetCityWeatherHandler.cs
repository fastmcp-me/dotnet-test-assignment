using MediatR;
using Microsoft.Extensions.Logging;
using Weather.Core.Exceptions;
using Weather.Core.Interfaces;

namespace Weather.UseCases.Forecast.GetCityWeather;

public class GetCityWeatherHandler(IWeatherForecast forecast, ILogger<GetCityWeatherHandler> logger) : IRequestHandler<GetCityWeatherRequest, string>
{
    public async Task<string> Handle(GetCityWeatherRequest request, CancellationToken cancellationToken)
    {
        var (city, countryCode, stateCode) = request;
    
        try
        {
            var weather = await forecast.GetCurrentWeatherByCityAsync(city, countryCode, stateCode, cancellationToken);

            return $"The current weather in {weather.City} is {weather.WeatherCondition} with a temperature of {weather.Temperature}°C. " +
                $"Data retrieved at {weather.Timestamp:HH:mm:ss} UTC.";

        }
        catch (Exception ex) when (ex is WeatherArgumentException)
        {
            logger.LogError(ex, "Error getting weather for {City}", city);

            return $"Error getting weather for {request.City}: {ex.Message}";
        }
	}
}
