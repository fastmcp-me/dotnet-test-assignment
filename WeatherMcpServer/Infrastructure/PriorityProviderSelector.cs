using Microsoft.Extensions.Options;
using WeatherMcpServer.Application.Interfaces;
using WeatherMcpServer.Domain;

namespace WeatherMcpServer.Infrastructure;

public class PriorityProviderSelector : IProviderSelector
{
    private readonly Dictionary<string, WeatherProviderConfig> _configs;

    public PriorityProviderSelector(IOptionsMonitor<Dictionary<string, WeatherProviderConfig>> configs)
    {
        _configs = configs.CurrentValue;
    }

    public IEnumerable<IWeatherProvider> Select(IEnumerable<IWeatherProvider> providers) =>
        providers.OrderBy(p => _configs[p.Name].Priority);
}
