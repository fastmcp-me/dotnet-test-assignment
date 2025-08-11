using System.ComponentModel;
using ModelContextProtocol.Server;
using WeatherMcpServer.Application.Interfaces;
using WeatherMcpServer.Domain;

namespace WeatherMcpServer.Tools;

public class WeatherTools(IWeatherMediator orchestrator)
{
    [McpServerTool]
    [Description("Describes random weather in the provided city.")]
    public async Task<Result<WeatherResult>> GetCityWeather(
        [Description("Name of the city to return weather for")] string city) =>
            await orchestrator.GetCurrentWeatherAsync(city);

    [McpServerTool]
    [Description("Gets weather forecast for the specified city.")]
    public async Task<Result<IEnumerable<WeatherResult>>> GetWeatherForecast(
        [Description("The city name to get weather forecast for")] string city,
        [Description("Number of days to forecast (1-5)")] int days = 3)
    {
         return await orchestrator.GetForecastAsync(city, days);
    }


}