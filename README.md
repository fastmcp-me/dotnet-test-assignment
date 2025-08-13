WeatherMcpServer
WeatherMcpServer is a Model Context Protocol (MCP) server that provides real-time weather data, forecasts, and alerts for any city in the world.
It integrates with the OpenWeather API and exposes weather functionality as MCP tools, making it easy to integrate into MCP-compatible clients.

Features
Current Weather – Get temperature, humidity, and description of current conditions.

Forecast – Retrieve multi-day weather forecasts.

Weather Alerts – Receive official weather alerts with severity levels (advisory, watch, warning).

Caching – Uses in-memory caching to reduce redundant API calls.

Error Handling – Graceful handling of service errors and network issues.

Integration Ready – Works as a standalone MCP server via stdio transport.

Architecture Overview
Presentation Layer
Hosts the MCP server and registers weather tools.

Application Layer
Contains the WeatherService orchestrating weather provider calls, caching, and combining results.

Infrastructure Layer
Implements IWeatherProvider using the OpenWeather REST API, mapping responses to domain models.

Domain Layer
Defines core entities like Location, WeatherInfo, WeatherForecast, and WeatherAlert.

MCP Tools
GetCurrentWeather(city, countryCode?) – Returns current weather conditions.

GetWeatherForecast(city, countryCode?, days) – Returns forecast for the specified number of days.

GetWeatherAlerts(city, countryCode?) – Returns any active weather alerts.
