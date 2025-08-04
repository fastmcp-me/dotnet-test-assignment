using System.Net.Http.Json;
using Domain.Common.DTOs.OpenWeatherMap;
using Domain.Common.Weathers;
using Domain.Exceptions;
using Infrastructure.Services.OpenWeatherMap.Models;
using Infrastructure.Services.OpenWeatherMap.Models.Alerts;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.OpenWeatherMap;

public class OpenWeatherClient : IWeatherClient
{
    #region DI
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string _apiUrl;

    public OpenWeatherClient(HttpClient http, IConfiguration cfg)
    {
        _http = http;
        _apiKey = cfg["OpenWeather:ApiKey"]!;
        _apiUrl = cfg["OpenWeather:ApiUrl"]!;
    }
    #endregion


    public async Task<WeatherDto> GetCurrentAsync(string city, string? countryCode, CancellationToken ct)
    {
        var q = countryCode is null ? city : $"{city},{countryCode}";
        var url = $"{_apiUrl}/weather?q={q}&appid={_apiKey}&units=metric";

        var res = await _http.GetFromJsonAsync<OW_Current>(url, ct)
                  ?? throw new WeatherNotFoundException(city);

        return new WeatherDto(
            tempC: res.Main.Temp,
            description: res.Weather[0].Description,
            timeUtc: DateTimeOffset.FromUnixTimeSeconds(res.Dt));
    }

    public async Task<string?> GetAlertsAsync(string city, string? countryCode, CancellationToken ct)
    {
        // 1. Сначала получаем координаты по названию города
        var q = countryCode is null ? city : $"{city},{countryCode}";
        var url = $"https://api.openweathermap.org/data/2.5/weather?q={q}&appid={_apiKey}";

        var res = await _http.GetFromJsonAsync<OW_Geo>(url, ct)
            ?? throw new WeatherNotFoundException(city);

        var lat = res.Coord.Lat;
        var lon = res.Coord.Lon;

        // 2. Делаем запрос в OneCall API
        var oneCallUrl = $"https://api.openweathermap.org/data/3.0/onecall?lat={lat}&lon={lon}&appid={_apiKey}&exclude=minutely,hourly,daily,current";

        var alertData = await _http.GetFromJsonAsync<OW_OneCall>(oneCallUrl, ct);

        if (alertData?.Alerts is null || alertData.Alerts.Count == 0)
            return null;

        var a = alertData.Alerts.First();

        return $"❗ Alert: {a.Event} from {a.Sender_name}\n{a.Description}\n(valid from {DateTimeOffset.FromUnixTimeSeconds(a.Start):g} to {DateTimeOffset.FromUnixTimeSeconds(a.End):g})";
    }

    public async Task<IEnumerable<ForecastDto>> GetForecastAsync(string city, string? country, int days, CancellationToken ct)
    {
        if (days < 1 || days > 5)
            throw new ArgumentOutOfRangeException(nameof(days), "OpenWeather позволяет максимум 5 дней прогноза");

        var q = country is null ? city : $"{city},{country}";
        var url = $"{_apiUrl}/forecast?q={q}&appid={_apiKey}&units=metric";

        var res = await _http.GetFromJsonAsync<OW_Forecast>(url, ct)
                  ?? throw new WeatherNotFoundException(city);

        // Фильтруем по дням, выбираем прогнозы ближе к полудню
        var grouped = res.List
            .Select(f => new ForecastDto(
                date: DateTimeOffset.FromUnixTimeSeconds(f.Dt),
                tempC: f.Main.Temp,
                description: f.Weather[0].Description))
            .GroupBy(x => x.Date.Date)
            .Take(days)
            .Select(g => g.OrderBy(x => Math.Abs((x.Date - x.Date.Date.AddHours(12)).TotalHours)).First());

        return grouped;
    }
}