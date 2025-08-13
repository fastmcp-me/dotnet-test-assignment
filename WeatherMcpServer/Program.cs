using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherMcpServer.Clients;
using WeatherMcpServer.Options;
using WeatherMcpServer.Services;
using WeatherMcpServer.Tools;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddOptions<OpenWeatherOptions>()
    .Bind(builder.Configuration.GetSection(nameof(OpenWeatherOptions)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Debug);

builder.Services.AddHttpClient<OpenWeatherHttpClient>((services, client) =>
{
    client.BaseAddress = new Uri(services.GetRequiredService<IOptions<OpenWeatherOptions>>().Value.BaseUrl);
});
builder.Services.AddTransient<OpenWeatherService>();
//builder.Services.AddTransient<WeatherTools>();

// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<WeatherTools>();

var host = builder.Build();

//var tool = host.Services.GetRequiredService<WeatherTools>();
//Console.WriteLine($"\n{await tool.GetCurrentWeather("Moscow", "ru")}\n");
//Console.WriteLine($"\n{await tool.GetWeatherForecast("Moscow", "ru")}\n");
//Console.WriteLine($"\n{await tool.GetWeatherAlerts("Moscow", "ru")}\n");

await host.RunAsync();