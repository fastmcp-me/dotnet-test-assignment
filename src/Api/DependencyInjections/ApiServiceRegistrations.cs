using Api.Tools;
using Domain.Common.Auth;

namespace Api.DependencyInjections;

public static class ApiServiceRegistrations
{
    public static IServiceCollection AddApiServices
        (this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();

        services.AddMcpServer().WithStdioServerTransport()
            .WithTools<RandomNumberTools>()
            .WithTools<WeatherTools>()
            .WithTools<GreetingTools>();

        return services;
    }
}