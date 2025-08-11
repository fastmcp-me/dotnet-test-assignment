using System.Net.Http.Json;
using System.Text.Json;
using WeatherMcpServer.Application.Interfaces;
using WeatherMcpServer.Domain;

namespace WeatherMcpServer.Infrastructure.Providers;

public abstract class WeatherProviderBase : IWeatherProvider
{
    protected readonly ICustomHttpClientFactory _clientFactory;
    protected readonly ILinkBuilder _linkBuilder;
    protected readonly string _baseUrl;
    protected readonly string _apiKey;

    public abstract string Name { get; }
    public abstract int Weight { get; }

    protected WeatherProviderBase(
        ICustomHttpClientFactory clientFactory,
        ILinkBuilder linkBuilder,
        string baseUrl,
        string apiKey)
    {
        _clientFactory = clientFactory;
        _linkBuilder = linkBuilder;
        _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
    }

    public abstract Task<WeatherResult> ParseCurrentAsync(JsonElement json, string location);
    public abstract Task<IEnumerable<WeatherResult>> ParseForecastAsync(JsonElement json, string location, int days);

    public async Task<WeatherResult> GetCurrentWeatherAsync(string location, CancellationToken ct = default)
    {
        var uri = BuildUri("weather", location);
        var json = await GetJsonAsync(uri, ct);
        return await ParseCurrentAsync(json, location);
    }

    public async Task<IEnumerable<WeatherResult>> GetForecastAsync(string location, int days = 3, CancellationToken ct = default)
    {
        var uri = BuildUri("forecast", location);
        var json = await GetJsonAsync(uri, ct);
        return await ParseForecastAsync(json, location, days);
    }

    private Uri BuildUri(string endpoint, string location)
    {
        var query = new Dictionary<string, string?>
        {
            ["q"] = location,
            ["appid"] = _apiKey,
            ["units"] = "metric"
        };
        return _linkBuilder.BuildUri(_baseUrl, new[] { endpoint }, query);
    }

    private async Task<JsonElement> GetJsonAsync(Uri uri, CancellationToken ct)
    {
        var client = _clientFactory.GetClient(Name);
        var response = await client.GetAsync(uri, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
    }
}
