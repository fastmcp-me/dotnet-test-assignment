using Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherAPI.Configuration;

namespace WeatherAPI;

public static class Program_Extension
{
	public static IHostApplicationBuilder AddWeatherFeature(this IHostApplicationBuilder builder)
	{
		builder.Services.Configure<OpenWeatherMapOptions>(
			builder.Configuration.GetSection(Constants.OpenWeatherMap)
		);

		builder.Services.AddHttpClient<IWeatherForecast, WeatherApi>()
			.ConfigureHttpClient(client =>
			{
				client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
			});

		return builder;
	}
}
