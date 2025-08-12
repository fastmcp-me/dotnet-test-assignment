using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Weather.Core.Interfaces;
using Weather.UseCases.Forecast.GetCityWeather;
namespace Weather.UseCases;

public static class UseCasesBuilderExtensions
{
	public static IHostApplicationBuilder AddWeatherUseCases(this IHostApplicationBuilder builder)
	{
		builder.Services.AddMediatR(cfg => {
			cfg.RegisterServicesFromAssemblyContaining<GetCityWeatherRequest>();
		});
		builder.Services.AddScoped<IUseCaseDispatcher, UseCaseDispatcher>();
	
		return builder;
	}
}
