using Application.Common.Behaviors;
using Application.Common.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.DependencyInjections;

public static class ApplicationServiceRegistrations
{
    public static IServiceCollection AddApplicationServices
        (this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        #region MediatR and FluentValidation
        services.AddFluentValidation(config =>
        {
            config.AutomaticValidationEnabled = false;
        });
        services.AddValidatorsFromAssembly(typeof(IAssemblyMarker).Assembly);
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(IAssemblyMarker).Assembly);
        });
        #endregion



        #region Behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        #endregion




        return services;
    }
}