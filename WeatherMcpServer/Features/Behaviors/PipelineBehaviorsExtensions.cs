using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace WeatherMcpServer.Features.Behaviors;

/// <summary>
/// Provides extension methods for adding core behaviors to the service collection.
/// </summary>
public static class PipelineBehaviorsExtensions
{
    /// <summary>
    /// Adds the core behaviors to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static void AddCoreBehaviors(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(WeatherServiceExceptionBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }
}
