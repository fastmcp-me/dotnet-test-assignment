# MCP Server

A production-ready MCP (Model Context Protocol) server that provides real-time weather data through AI assistants like Claude. Built with .NET 8		
		
- **Real Weather Data**: Integration with OpenWeatherMap API for accurate, current weather information
- **Current Weather**: Get real-time weather conditions for any city worldwide
- **Weather Forecast**: Provides up to 5-day weather forecasts with detailed daily information


## Prerequisites
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
$env:OPENWEATHER_API_KEY="your-api-key-here"

# Windows Command Prompt
set OPENWEATHER_API_KEY=your-api-key-here

# Linux/macOS
export OPENWEATHER_API_KEY="your-api-key-here"
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

**Example Output:**
```
{
    "success": true,
    "value": {
        "providerName": "OpenWeather",
        "location": "Miami",
        "timestamp": "2025-08-11T07:59:51.8825174+00:00",
        "description": "few clouds",
        "temperatureC": 26.61,
        "isFallback": false
    }
}
```
		
### 2. GetWeatherForecast
Gets weather forecast for a specified city and days.
This will return you the weather forecast for the city in the format: number of days requested and for each day the average temperature for morning, afternoon, evening

**Parameters:**
- `city` (required): The city name to get weather forecast for
- `days` (required): Number of days to forecast (1-5, default: 3)

**Example Output:**
```
{
    "success": true,
    "value": [
        {
            "providerName": "OpenWeather",
            "location": "Beijing",
            "timestamp": "2025-08-11T06:00:00+05:00",
            "description": "Morning",
            "temperatureC": 31.9,
            "isFallback": false
        },
        {
            "providerName": "OpenWeather",
            "location": "Beijing",
            "timestamp": "2025-08-11T12:00:00+05:00",
            "description": "Day",
            "temperatureC": 29.1,
            "isFallback": false
        },
        {
            "providerName": "OpenWeather",
            "location": "Beijing",
            "timestamp": "2025-08-11T18:00:00+05:00",
            "description": "Evening",
            "temperatureC": 23.4,
            "isFallback": false
        },
        {
            "providerName": "OpenWeather",
            "location": "Beijing",
            "timestamp": "2025-08-12T06:00:00+05:00",
            "description": "Morning",
            "temperatureC": 29.7,
            "isFallback": false
        },
        {
            "providerName": "OpenWeather",
            "location": "Beijing",
            "timestamp": "2025-08-12T12:00:00+05:00",
            "description": "Day",
            "temperatureC": 22.6,
            "isFallback": false
        },
        {
            "providerName": "OpenWeather",
            "location": "Beijing",
            "timestamp": "2025-08-12T18:00:00+05:00",
            "description": "Evening",
            "temperatureC": 21.4,
            "isFallback": false
        },
        {
            "providerName": "OpenWeather",
            "location": "Beijing",
            "timestamp": "2025-08-13T06:00:00+05:00",
            "description": "Morning",
            "temperatureC": 28,
            "isFallback": false
        },
        {
            "providerName": "OpenWeather",
            "location": "Beijing",
            "timestamp": "2025-08-13T12:00:00+05:00",
            "description": "Day",
            "temperatureC": 24.8,
            "isFallback": false
        },
        {
            "providerName": "OpenWeather",
            "location": "Beijing",
            "timestamp": "2025-08-13T18:00:00+05:00",
            "description": "Evening",
            "temperatureC": 22.5,
            "isFallback": false
        },
        {
            "providerName": "OpenWeather",
            "location": "Beijing",
            "timestamp": "2025-08-14T06:00:00+05:00",
            "description": "Morning",
            "temperatureC": 33.3,
            "isFallback": false
        },
        {
            "providerName": "OpenWeather",
            "location": "Beijing",
            "timestamp": "2025-08-14T12:00:00+05:00",
            "description": "Day",
            "temperatureC": 26.4,
            "isFallback": false
        },
        {
            "providerName": "OpenWeather",
            "location": "Beijing",
            "timestamp": "2025-08-14T18:00:00+05:00",
            "description": "Evening",
            "temperatureC": 23.2,
            "isFallback": false
        },
        {
            "providerName": "OpenWeather",
            "location": "Beijing",
            "timestamp": "2025-08-15T06:00:00+05:00",
            "description": "Morning",
            "temperatureC": 33.6,
            "isFallback": false
        },
        {
            "providerName": "OpenWeather",
            "location": "Beijing",
            "timestamp": "2025-08-15T12:00:00+05:00",
            "description": "Day",
            "temperatureC": 29,
            "isFallback": false
        },
        {
            "providerName": "OpenWeather",
            "location": "Beijing",
            "timestamp": "2025-08-15T18:00:00+05:00",
            "description": "Evening",
            "temperatureC": 25.7,
            "isFallback": false
        }
    ]
}
```


## Configuration Options

The server can be configured via `appsettings.json`:

```json
{
  "WeatherProviders": {
    "OpenWeather": {
      "BaseUrl": "https://api.openweathermap.org/data/2.5/", - base url of weather provider
      "ApiKeyEnvVar": "OPENWEATHER_API_KEY", - environment variable name for API key
      "Weight": 100, - weight for provider selection (higher means more preferred)
      "Priority": 1 - priority for provider selection (lower means higher priority)
    },
    "MockProvider": { - Mock provider in case none of the providers could give an answer but we have to return some value
      "BaseUrl": "https://mock.weather.local/", - base url of mock provider
      "ApiKeyEnvVar": "MOCK_API_KEY", 
      "Weight": 50,
      "Priority": 2
    }
  },
  "HttpClientPolicies": {
    "RetryCount": 3,
    "RetryBaseDelaySeconds": 2,
    "CircuitBreakerAllowedErrors": 5,
    "CircuitBreakerDurationSeconds": 30
  }
}
```

## Unit tests
An alternative is to run manual tests that will send real requests to the server and receive data from it
To do this, simply go to the project with tests, set the necessary points for debugging and run the tests.



## schema

+-------------------+         +-------------------+         +-------------------+
|                   |         |                   |         |                   |
| WeatherMcpServer  |<------->| ModelContextProto |<------->| McpTestClient     |
| (Host/Server)     |  MCP    | col.Client        |  MCP    | (Client)          |
|                   | Protocol|                   | Protocol|                   |
+-------------------+         +-------------------+         +-------------------+
        |                                                       
        | Hosts                                                    
        v                                                        
+-------------------+                                                
| ASP.NET Host      |                                                
| (Generic Host)    |                                                
+-------------------+                                                
        |                                                        
        | DI/Service Registration                                 
        v                                                        
+-------------------+                                                
| ServiceCollection |                                                
+-------------------+                                                
        |                                                        
        | Registers                                                
        v                                                        
+-------------------+                                                
| Weather Tools     |<-------------------+                          
| (WeatherTools,    |                    |                          
|  RandomNumberTools|                    |                          
+-------------------+                    |                          
        ^                                |                          
        | Uses                           |                          
        |                                |                          
+-------------------+                    |                          
| IWeatherMediator  |<-------------------+                          
| (decorated by     |                    |                          
| Logging decorator)|                    |                          
+-------------------+                    |                          
        ^                                |                          
        | Uses                           |                          
+-------------------+                    |                          
| IWeatherProvider  |<-------------------+                          
| (OpenWeather,     |                    |                          
|  Fallback,        |                    |                          
|  Logging decorator)|                   |                          
+-------------------+                    |                          
        ^                                |                          
        | Uses                           |                          
+-------------------+                    |                          
| IProviderSelector |                    |                          
+-------------------+                    |                          
        ^                                |                          
        | Uses                           |                          
+-------------------+                    |                          
| IHttpPolicyFactory|                    |                          
+-------------------+                    |                          
        ^                                |                          
        | Uses                           |                          
+-------------------+                    |                          
| HttpClientFactory |                    |                          
+-------------------+                    |                          
        ^                                |                          
        | Uses                           |                          
+-------------------+                    |                          
| RateLimiter       |                    |                          
+-------------------+                    |                          
        ^                                |                          
        | Reads from                     |                          
+-------------------+                    |                          
| appsettings.json   |<------------------+                          
+-------------------+                                                
