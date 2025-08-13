using Microsoft.Extensions.Configuration;

namespace IntegrationTests;

public abstract class TestBase : IDisposable
{
    protected readonly IConfiguration _configuration;
    protected readonly HttpClient _httpClient;

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
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
