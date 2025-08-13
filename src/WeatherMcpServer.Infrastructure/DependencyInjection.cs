using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherMcpServer.Domain.LocationAggregate;
using WeatherMcpServer.Infrastructure.OpenWeather;

namespace WeatherMcpServer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var baseAddress = configuration["OpenWeather:BaseAddress"]
            ?? throw new ArgumentNullException("OpenWeather API key is missing in configuration.");
    
        services.AddHttpClient<OpenWeatherProvider>((sp, client) =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddScoped<IWeatherProvider, OpenWeatherProvider>();

        return services;
    }
}
