using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherMcpServer.Models;
using WeatherMcpServer.Services;
using Microsoft.Extensions.Options;


var builder = Host.CreateApplicationBuilder(args);
var configuration = builder.Configuration;

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Configuration.GetSection("OpenWeatherMap").Get<OpenWeatherMapConfig>();

builder.Services.AddHttpClient<IWeatherService, WeatherService>((provider, client) =>
{
    var configuration = provider.GetRequiredService<IOptions<OpenWeatherMapConfig>>().Value;
    var logger = provider.GetRequiredService<ILogger<IWeatherService>>();
    client.BaseAddress = new Uri(configuration.BaseUrl);
});


builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<WeatherTools>();

await builder.Build().RunAsync();
