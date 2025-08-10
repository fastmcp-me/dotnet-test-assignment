using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using WeatherMcpServer.Application.Interfaces;
using WeatherMcpServer.Domain;
using WeatherMcpServer.Infrastructure.DelegatingHandlers;

namespace WeatherMcpServer.Infrastructure.Factories;

public class CustomHttpClientFactory : ICustomHttpClientFactory
{
    private readonly ConcurrentDictionary<string, HttpClient> _clients = new();
    private readonly Dictionary<string, WeatherProviderConfig> _configs;
    private readonly IHttpPolicyFactory _policyFactory;
    private readonly ILogger<CustomHttpClientFactory> _logger;

    public CustomHttpClientFactory(
        IHttpPolicyFactory policyFactory,
        IOptionsMonitor<Dictionary<string, WeatherProviderConfig>> configs,
        ILogger<CustomHttpClientFactory> logger)
    {
        _policyFactory = policyFactory;
        _logger = logger;
        _configs = configs.CurrentValue;

        configs.OnChange(RebuildClients);
        RebuildClients(_configs);
    }

    public HttpClient GetClient(string providerName) =>
        _clients.GetValueOrDefault(providerName)
        ?? throw new KeyNotFoundException($"No HttpClient for provider {providerName}");

    public HttpClient GetPreferredClient()
    {
        var bestProvider = _configs.OrderBy(kv => kv.Value.Priority).First().Key;
        return GetClient(bestProvider);
    }

    private void RebuildClients(Dictionary<string, WeatherProviderConfig> configs)
    {
        foreach (var (name, cfg) in configs)
        {
            var handler = new PolicyHttpMessageHandler(_policyFactory.CreateCombinedPolicy())
            {
                InnerHandler = new HttpClientHandler()
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(cfg.BaseUrl),
                Timeout = TimeSpan.FromSeconds(10)
            };
            client.DefaultRequestHeaders.Add("User-Agent", "WeatherMcpServer");

            _clients[name] = client;
        }
    }
}

