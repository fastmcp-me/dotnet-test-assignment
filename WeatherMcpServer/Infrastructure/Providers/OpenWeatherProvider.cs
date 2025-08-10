using Microsoft.Extensions.Options;
using System.Text.Json;
using WeatherMcpServer.Application.Interfaces;
using WeatherMcpServer.Domain;

namespace WeatherMcpServer.Infrastructure.Providers;

public class OpenWeatherProvider : WeatherProviderBase
{
    public override string Name => "OpenWeather";
    public override int Weight { get; }

    public OpenWeatherProvider(
        ICustomHttpClientFactory clientFactory,
        ILinkBuilder linkBuilder,
        IOptionsMonitor<Dictionary<string, WeatherProviderConfig>> configs)
        : base(clientFactory, linkBuilder,
               configs.CurrentValue["OpenWeather"].BaseUrl!,
               Environment.GetEnvironmentVariable(configs.CurrentValue["OpenWeather"].ApiKeyEnvVar!)!)
    {
        Weight = configs.CurrentValue["OpenWeather"].Weight;
    }

    public override Task<WeatherResult> ParseCurrentAsync(JsonElement json, string location)
    {
        var desc = json.GetProperty("weather")[0].GetProperty("description").GetString();
        var temp = json.GetProperty("main").GetProperty("temp").GetDouble();

        return Task.FromResult(new WeatherResult
        {
            ProviderName = Name,
            Location = location,
            Timestamp = DateTimeOffset.UtcNow,
            Description = desc ?? string.Empty,
            TemperatureC = temp
        });
    }

    public override Task<IEnumerable<WeatherResult>> ParseForecastAsync(JsonElement json, string location, int days)
    {
        var list = json.GetProperty("list")
            .EnumerateArray()
            .Take(days)
            .Select(item =>
            {
                var dateText = item.GetProperty("dt_txt").GetString();
                var desc = item.GetProperty("weather")[0].GetProperty("description").GetString();
                var temp = item.GetProperty("main").GetProperty("temp").GetDouble();

                return new WeatherResult
                {
                    ProviderName = Name,
                    Location = location,
                    Timestamp = DateTimeOffset.Parse(dateText!),
                    Description = desc ?? string.Empty,
                    TemperatureC = temp
                };
            });

        return Task.FromResult(list);
    }
}

