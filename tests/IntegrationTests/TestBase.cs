using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using WeatherMcpServer.Application.Abstractions;
using WeatherMcpServer.Infrastructure.Cache;

namespace IntegrationTests;

public abstract class TestBase : IDisposable
{
    protected readonly IConfiguration _configuration;
    protected readonly HttpClient _httpClient;
    protected readonly ICacheService _cacheService;

    protected TestBase()
    {
        var config = new ConfigurationBuilder()
           .SetBasePath(AppContext.BaseDirectory)
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
           .Build();

        _configuration = config;

        var baseUriStr = _configuration["OpenWeather:BaseUrl"]!;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUriStr)
        };

        _cacheService = new MemoryCacheService(new MemoryCache(new MemoryCacheOptions()));

    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
