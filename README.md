# 🌤️ Real Weather MCP Server (FastMCP Assignment)

This is a .NET 10 MCP Server that provides **real-time weather data** using [WeatherAPI.com](https://www.weatherapi.com/) via the [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) standard.

It was created as part of the FastMCP assignment and showcases full integration with the `.NET MCP Server` framework, weather tooling, structured responses, and rich test coverage.

---

## ✨ Features Implemented

✅ **Three Fully Functional MCP Tools**  
- `GetCurrentWeather(city, countryCode?)`: returns live weather info  
- `GetWeatherForecast(city, countryCode?)`: shows a 5-day forecast  
- `GetWeatherAlerts(city, countryCode?)`: fetches alerts (bonus tool)

✅ **Real API Integration**  
- Uses **WeatherAPI** with API key using it directly in ai agent or via appsettings.json

✅ **MCP Server Tooling**  
- All tools marked with `[McpServerTool]`  
- Detailed `[Description]` attributes for parameters and methods  
- Auto-discoverable from MCP clients (Github Copilot, etc.)

✅ **Error Handling**  
- Invalid city names, missing input, failed API calls, invalid city — all handled with user-friendly responses  
- Fallback for missing alerts in free tier

✅ **Logging**  
- Integrated with `ILogger<T>`

---

## 🚀 Setup Instructions

[All installation, configuration and usage instructions are here.](/WeatherMcpServer/README.md)

## Documentation (architecture overview)

The main focus in the design of the architecture and structure of the project was on **scaling**:
- adding support for new functions of already integrated services
- integrating new services.

An approach with direct interaction with the json response of the service was also chosen, since it is easier to support json schemas than a large number of data transfer objects.

### Layers
**1. Configuration layer**
- `OpenWeatherOptions.cs` contains `BaseUrl` and `ApiKey` with validation via DataAnnotations.

**2. Infrastructure layer**
- `OpenWeatherHttpClient.cs`: wrapper over `HttpClient`, with logging of requests and responses, error handling and JSON deserialization.

**3. Application / Service layer**
- `OpenWeatherService.cs`: responsible for URL generation, API calls (current weather, forecast, alerts), obtaining coordinates via the geocode API. Also validation of input parameters and logging.

**4. Formatters and presenters**
- `OpenWeatherFormatter.cs` and `OpenWeatherPresenter.cs`: validation of JSON structure via schemas, data extraction and transformation (`GeoCoordinate`, forecast/alerts description).

- `DateTimeExtension.cs`: extension for converting Unix time to DateTime.

**5. Presentation / MCP server tools**
- `WeatherTools.cs`: public interface of MCP API— methods `GetCurrentWeather`, `GetWeatherForecast`, `GetWeatherAlerts`, with logging, error handling, returning human-readable strings.

**6. Hosting and DI**
- `Program.cs`: DI setup, `OpenWeatherOptions` setup, `HttpClient`, `OpenWeatherService`, `WeatherTools` registration, MCP server launch via `WithTools<WeatherTools>()`.

This approach corresponds to the classic N-Tier or Clean Architecture: configuration → infrastructure → business logic → formatting and presentation → interface.

👨‍💻 Author
Built by OKtol <br>
Repo: https://github.com/OKtol/dotnet-test-assignment
