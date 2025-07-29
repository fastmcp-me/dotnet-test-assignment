using MediatR;
using Microsoft.Extensions.Options;
using WeatherMcpServer.Business.Configuration;

namespace WeatherMcpServer.Business.Queries.GetCityWeather;

public class GetCityWeatherQueryHandler : IRequestHandler<GetCityWeatherQuery, GetCityWeatherResponse>
{
    private readonly WeatherOptions _weatherOptions;

    public GetCityWeatherQueryHandler(IOptions<WeatherOptions> weatherOptions)
    {
        _weatherOptions = weatherOptions.Value;
    }

    public Task<GetCityWeatherResponse> Handle(GetCityWeatherQuery request, CancellationToken cancellationToken)
    {
        var weatherChoices = GetWeatherChoices();
        var selectedWeather = SelectRandomWeather(weatherChoices);
        var weatherDescription = $"The weather in {request.City} is {selectedWeather}.";
        
        return Task.FromResult(new GetCityWeatherResponse(weatherDescription));
    }

    private string[] GetWeatherChoices()
    {
        // First check if choices are configured
        if (_weatherOptions.Choices.Length > 0)
        {
            return _weatherOptions.Choices;
        }

        // Then check environment variable
        var environmentWeather = Environment.GetEnvironmentVariable("WEATHER_CHOICES");
        if (!string.IsNullOrWhiteSpace(environmentWeather))
        {
            return environmentWeather.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        // Finally use default
        return _weatherOptions.DefaultChoices.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static string SelectRandomWeather(string[] weatherChoices)
    {
        var selectedIndex = Random.Shared.Next(0, weatherChoices.Length);
        return weatherChoices[selectedIndex];
    }
}