using Domain.Common.Times;
using Domain.Common.Weathers;
using Infrastructure.Services.OpenWeatherMap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.DependencyInjections;

public static class InfrastructureServiceRegistrations
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddHttpClient<IWeatherClient, OpenWeatherClient>();
        services.AddTransient<ITimeProvider, DefaultTimeProvider>();

        return services;
    }
}