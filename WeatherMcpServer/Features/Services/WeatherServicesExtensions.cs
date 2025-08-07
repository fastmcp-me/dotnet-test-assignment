using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherMcpServer.Configurations;
using WeatherMcpServer.Features.Services.Interfaces;

namespace WeatherMcpServer.Features.Services;

/// <summary>
/// Provides extension methods for adding weather services to the service collection.
/// </summary>
internal static class WeatherServicesExtensions
{
    /// <summary>
    /// Adds the weather service to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    public static void AddWeatherService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<IWeatherService, OpenWeatherMapService>((provider, httpClient) =>
        {
            var configuration = provider.GetRequiredService<IOptions<WeatherApiConfiguration>>().Value;
            var logger = provider.GetRequiredService<ILogger<OpenWeatherMapService>>();
            logger.LogInformation("Units: {units}", configuration.Units);
            httpClient.BaseAddress = new Uri(configuration.Url);
        });
    }
}
