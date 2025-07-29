# Real Weather MCP Server

A production-ready MCP (Model Context Protocol) server that provides real-time weather data through AI assistants like Claude. Built with .NET 8, following best practices including CQRS, MediatR, FluentValidation, and SOLID principles.

## Features

- **Real Weather Data**: Integration with OpenWeatherMap API for accurate, current weather information
- **Current Weather**: Get real-time weather conditions for any city worldwide
- **Weather Forecast**: Provides up to 5-day weather forecasts with detailed daily information
- **Weather Alerts**: Support for weather warnings and alerts (requires paid API subscription)
- **Robust Error Handling**: Comprehensive exception handling and validation
- **Best Practices**: Implements CQRS pattern, dependency injection, and clean architecture

## Prerequisites

- .NET 8.0 or later
- OpenWeatherMap API key ([Get free API key](https://openweathermap.org/api))
- MCP-compatible AI assistant (e.g., Claude Desktop)

## Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd WeatherMcpServer
```

### 2. Configure API Key

You have three options to configure your OpenWeatherMap API key:

#### Option A: Environment Variable (Recommended)
```bash
# Windows PowerShell
$env:OpenWeatherMap__ApiKey="your-api-key-here"

# Windows Command Prompt
set OpenWeatherMap__ApiKey=your-api-key-here

# Linux/macOS
export OpenWeatherMap__ApiKey="your-api-key-here"
```

#### Option B: appsettings.json
Edit `appsettings.json` and add your API key:
```json
{
  "OpenWeatherMap": {
    "ApiKey": "your-api-key-here"
  }
}
```

#### Option C: User Secrets (Development)
```bash
dotnet user-secrets init
dotnet user-secrets set "OpenWeatherMap:ApiKey" "your-api-key-here"
```

### 3. Build the Project

```bash
dotnet build
```

### 4. Run the Server

```bash
dotnet run
```

The server will start and listen for MCP protocol messages via stdio.

## MCP Tools Available

### 1. GetCurrentWeather
Gets current weather conditions for a specified city.

**Parameters:**
- `city` (required): The city name to get weather for
- `countryCode` (optional): Country code (e.g., 'US', 'UK', 'FR')

**Example Output:**
```
📍 Current Weather in London, GB
🌡️ Temperature: 15.2°C (feels like 14.8°C)
☁️ Condition: Clouds - overcast clouds
💧 Humidity: 72%
💨 Wind Speed: 3.5 m/s
🔵 Pressure: 1015 hPa
⏰ Last Updated: 2024-01-29 14:30:00
```

### 2. GetWeatherForecast
Gets weather forecast for a specified city.

**Parameters:**
- `city` (required): The city name to get weather forecast for
- `countryCode` (optional): Country code (e.g., 'US', 'UK', 'FR')
- `days` (optional): Number of days to forecast (1-5, default: 3)

**Example Output:**
```
📅 Weather Forecast for Paris, FR

📆 Monday, January 29
   🌡️ Temperature: 12.5°C (Min: 8.2°C, Max: 16.3°C)
   ☁️ Condition: Clear - clear sky
   💧 Humidity: 65% | Precipitation: 0%
   💨 Wind Speed: 2.8 m/s

📆 Tuesday, January 30
   🌡️ Temperature: 14.1°C (Min: 10.5°C, Max: 17.8°C)
   ☁️ Condition: Clouds - few clouds
   💧 Humidity: 70% | Precipitation: 10%
   💨 Wind Speed: 3.2 m/s
```

### 3. GetWeatherAlerts
Gets weather alerts and warnings for a location.

**Parameters:**
- `city` (required): The city name to get weather alerts for
- `countryCode` (optional): Country code (e.g., 'US', 'UK', 'FR')

**Note:** This feature requires a paid OpenWeatherMap subscription. With the free tier, it will return an informational message.

## Architecture Overview

The server follows clean architecture principles with the following structure:

```
WeatherMcpServer/
├── Business/                    # Core business logic
│   ├── Configuration/          # Configuration models
│   ├── Infrastructure/         # Cross-cutting concerns
│   │   ├── Behaviors/         # MediatR pipeline behaviors
│   │   └── Exceptions/        # Custom exceptions
│   ├── Interfaces/            # Abstractions
│   ├── Models/                # Domain models
│   └── Queries/               # CQRS queries
│       ├── GetCurrentWeather/
│       ├── GetWeatherForecast/
│       └── GetWeatherAlerts/
├── Integrations/              # External service integrations
│   └── OpenWeatherMap/        # OpenWeatherMap API implementation
│       ├── Models/           # API response models
│       └── Services/         # Service implementation
└── Tools/                     # MCP tool implementations
```

## Configuration Options

The server can be configured via `appsettings.json`:

```json
{
  "OpenWeatherMap": {
    "ApiKey": "",                                    // Your API key
    "BaseUrl": "https://api.openweathermap.org/data/2.5", // API base URL
    "Units": "metric",                               // Units: metric, imperial, standard
    "TimeoutSeconds": 30                             // HTTP timeout
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

## Error Handling

The server implements comprehensive error handling:

- **Validation Errors**: Input validation using FluentValidation
- **API Errors**: Graceful handling of API failures with meaningful error messages
- **Network Errors**: Timeout and connection error handling
- **Business Logic Errors**: Custom exceptions for business rule violations

## Testing the Server

You can test the server using the MCP inspector or by integrating it with Claude Desktop.

### Using with Claude Desktop

1. Add the server to your Claude Desktop configuration:
```json
{
  "mcpServers": {
    "weather": {
      "command": "dotnet",
      "args": ["run", "--project", "path/to/WeatherMcpServer"],
      "env": {
        "OpenWeatherMap__ApiKey": "your-api-key-here"
      }
    }
  }
}
```

2. Restart Claude Desktop
3. The weather tools will be available in your conversations

## Development

### Adding New Weather Providers

1. Implement the `IWeatherProvider` interface
2. Register your implementation in `Program.cs`
3. Update configuration as needed

### Extending Functionality

The architecture supports easy extension through:
- Adding new queries/commands using MediatR
- Implementing new validators using FluentValidation
- Adding new MCP tools in the Tools folder

## Troubleshooting

### Common Issues

1. **"API key is not configured"**
   - Ensure your API key is set correctly in environment variables or configuration

2. **"City not found"**
   - Verify the city name spelling
   - Try adding the country code (e.g., "London,UK")

3. **HTTP timeouts**
   - Check your internet connection
   - Increase `TimeoutSeconds` in configuration

### Logging

Logs are written to stderr (stdout is reserved for MCP protocol). To see detailed logs:
```bash
dotnet run --property:LogLevel=Debug
```

## License

This project is provided as a test assignment for FastMCP.me.

## Acknowledgments

- Built with the .NET MCP SDK
- Weather data provided by OpenWeatherMap API
- Implements Model Context Protocol (MCP) specification
