using Weather.Core;
using Weather.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Weather.Infrastructure;

public static class InfrastructureBuilderExtensions
{
	public static IHostApplicationBuilder AddWeatherFeature(this IHostApplicationBuilder builder)
	{
		builder.Services.Configure<WeatherConfiguration>(
			builder.Configuration.GetSection(Constants.OpenWeatherMap)
		);

		builder.Services.AddHttpClient<IWeatherForecast, WeatherApi>()
			.ConfigureHttpClient((sp, client) =>
			{
				var options = sp.GetRequiredService<IOptions<WeatherConfiguration>>().Value;
				client.BaseAddress = new Uri(options.BaseUrl);
			});

		return builder;
	}
}
