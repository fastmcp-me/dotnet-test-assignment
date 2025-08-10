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
        var groupedByDate = new Dictionary<DateTime, (double morningSum, int morningCount, double daySum, int dayCount, double eveningSum, int eveningCount)>();

        foreach (var item in json.GetProperty("list").EnumerateArray())
        {
            var dt = DateTime.Parse(item.GetProperty("dt_txt").GetString()!);
            var temp = item.GetProperty("main").GetProperty("temp").GetDouble();
            var date = dt.Date;
            var time = dt.TimeOfDay;

            if (!groupedByDate.TryGetValue(date, out var stats))
                stats = (0, 0, 0, 0, 0, 0);

            if (time >= TimeSpan.FromHours(6) && time < TimeSpan.FromHours(12))
            {
                stats.morningSum += temp;
                stats.morningCount++;
            }
            else if (time >= TimeSpan.FromHours(12) && time < TimeSpan.FromHours(18))
            {
                stats.daySum += temp;
                stats.dayCount++;
            }
            else if (time >= TimeSpan.FromHours(18) && time < TimeSpan.FromHours(24))
            {
                stats.eveningSum += temp;
                stats.eveningCount++;
            }

            groupedByDate[date] = stats;
        }

        var results = groupedByDate
            .OrderBy(kvp => kvp.Key)
            .Take(days)
            .SelectMany(kvp =>
            {
                var (morningSum, morningCount, daySum, dayCount, eveningSum, eveningCount) = kvp.Value;

                return new[]
                {
                    new WeatherResult
                    {
                        ProviderName = Name,
                        Location = location,
                        Timestamp = new DateTimeOffset(kvp.Key.AddHours(6)),
                        Description = "Morning",
                        TemperatureC = Math.Round(morningCount > 0 ? morningSum / morningCount : 0, 1),
                    },
                    new WeatherResult
                    {
                        ProviderName = Name,
                        Location = location,
                        Timestamp = new DateTimeOffset(kvp.Key.AddHours(12)),
                        Description = "Day",
                        TemperatureC = Math.Round(dayCount > 0 ? daySum / dayCount : 0, 1),
                    },
                    new WeatherResult
                    {
                        ProviderName = Name,
                        Location = location,
                        Timestamp = new DateTimeOffset(kvp.Key.AddHours(18)),
                        Description = "Evening",
                        TemperatureC = Math.Round(eveningCount > 0 ? eveningSum / eveningCount : 0, 1),
                    }
                };
            })
            .ToList();

        return Task.FromResult<IEnumerable<WeatherResult>>(results);
    }

}

