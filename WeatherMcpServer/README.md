# Weather MCP Server

Real-time weather data MCP server using OpenWeatherMap API.

## Features

- Current weather conditions
- Weather forecasts (up to 8 days)
- Weather alerts and warnings
- Global city support with geocoding
- Multiple units (metric, imperial, kelvin)
- Multiple languages

## Project Structure

```
WeatherMcpServer/
├── Commands/
│   └── GetCurrentWeatherCommand.cs          # All 3 command definitions
├── Handlers/
│   ├── GetCurrentWeatherHandler.cs          # Current weather handler
│   ├── GetWeatherForecastHandler.cs         # Forecast handler
│   └── GetWeatherAlertsHandler.cs           # Alerts handler
├── Models/
│   └── WeatherModels.cs                     # API response models
├── Services/
│   ├── IWeatherService.cs                   # Weather service interface
│   └── WeatherService.cs                    # OpenWeatherMap API client
├── Tools/
│   └── WeatherTools.cs                      # MCP tools (public API)
├── Program.cs                               # Application entry point
├── WeatherMcpServer.csproj                  # Project configuration
├── .env.example                             # Environment variables template
└── README.md                                # Project documentation
```

## Prerequisites

- .NET 8.0 or later
- OpenWeatherMap API key (free tier available)

## Setup

1. Get API key from [OpenWeatherMap](https://openweathermap.org/api)

2. Configure API key:
   ```bash
   # Copy example file and update with your API key
   cp .env.example .env
   # Edit .env file with your actual API key
   ```

3. Build project:
   ```bash
   dotnet build
   ```

## Usage

### Local Development

Configure in your IDE's MCP settings:

```json
{
  "servers": {
    "WeatherMcpServer": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "path/to/WeatherMcpServer"
      ]
    }
  }
}
```

### Available Tools

#### GetCurrentWeather
Gets current weather conditions for a city.

**Parameters:**
- `city` (required): City name
- `countryCode` (optional): Country code (e.g., "US", "UK")
- `units` (optional): "metric", "imperial", "kelvin" (default: "metric")
- `language` (optional): Language code (default: "en")

#### GetWeatherForecast
Gets weather forecast for a city.

**Parameters:**
- `city` (required): City name
- `countryCode` (optional): Country code
- `days` (optional): Number of forecast days 1-8 (default: 5)
- `units` (optional): Temperature units (default: "metric")
- `language` (optional): Language code (default: "en")

#### GetWeatherAlerts
Gets weather alerts and warnings for a city.

**Parameters:**
- `city` (required): City name
- `countryCode` (optional): Country code
- `language` (optional): Language code (default: "en")

## Testing

Run tests:
```bash
dotnet test
```

## Publishing

1. Update package metadata in `WeatherMcpServer.csproj`
2. Update `.mcp/server.json` configuration
3. Create package:
   ```bash
   dotnet pack -c Release
   ```
4. Publish to NuGet:
   ```bash
   dotnet nuget push bin/Release/*.nupkg --api-key <key> --source https://api.nuget.org/v3/index.json
   ```

## Using from NuGet

After publishing, configure with `dnx`:

```json
{
  "servers": {
    "WeatherMcpServer": {
      "type": "stdio",
      "command": "dnx",
      "args": [
        "YourPackageId",
        "--version",
        "1.0.0",
        "--yes"
      ]
    }
  }
}
```

## Environment Variables

- `OPENWEATHERMAP_API_KEY`: Required API key for OpenWeatherMap

## Dependencies

- ModelContextProtocol: MCP server framework
- MediatR: CQRS implementation
- Serilog: Structured logging
- DotNetEnv: Environment variable loading