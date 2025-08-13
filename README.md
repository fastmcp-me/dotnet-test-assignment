# 🌦 WeatherMcpServer

**WeatherMcpServer** is a **Model Context Protocol (MCP) server** that provides real-time weather data, multi-day forecasts, and official alerts for cities worldwide.  
It integrates with the **OpenWeather API** and exposes weather functionality as **MCP tools** for use in any MCP-compatible client.

---

## ✨ Features

- **🌡 Current Weather** – Temperature, humidity, and condition description.
- **📅 Multi-Day Forecasts** – Configurable number of days.
- **🚨 Weather Alerts** – Official alerts with severity levels: *Advisory*, *Watch*, *Warning*.
- **⚡ Caching** – In-memory cache to reduce redundant API calls.
- **🛡 Error Handling** – Resilient to network failures and API issues.
- **🔌 MCP Integration** – Runs as a standalone MCP server over `stdio`.

---

## 🏗 Architecture


- **Presentation Layer**  
  Runs the MCP server and registers weather tools.
  
- **Application Layer**  
  Orchestrates provider calls, applies caching, and combines results.
  
- **Infrastructure Layer**  
  Implements `IWeatherProvider` using OpenWeather REST API.
  
- **Domain Layer**  
  Defines `Location`, `WeatherInfo`, `WeatherForecast`, `WeatherAlert`, etc.

---

## 🛠 MCP Tools

| Tool Name            | Description |
|----------------------|-------------|
| `GetCurrentWeather`  | Returns current temperature, humidity, and conditions. |
| `GetWeatherForecast` | Returns weather forecast for a given number of days. |
| `GetWeatherAlerts`   | Returns active weather alerts with severity info. |

---

## ⚙️ Requirements

- [.NET 8.0+](https://dotnet.microsoft.com/en-us/download)
- [OpenWeather API key](https://openweathermap.org/api) (free or paid)
- MCP-compatible client for integration

---

## 🔑 Configuration

Add your OpenWeather API key to `appsettings.json`:

```json
{
  "OpenWeather": {
    "ApiKey": "YOUR_API_KEY_HERE"
  }
}
