using WeatherMcpServer.Business.Models;

namespace WeatherMcpServer.Business.Infrastructure.Formatters;

public interface IWeatherResponseFormatter
{
    string FormatCurrentWeather(CurrentWeatherData weather);
    string FormatWeatherForecast(WeatherForecastData forecast);
    string FormatWeatherAlerts(WeatherAlertsData alerts);
}