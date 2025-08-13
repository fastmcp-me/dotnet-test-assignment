namespace WeatherMcpServer.Domain.LocationAggregate;

public class LocationWeather // Aggregate Root
{
    public Location Location { get; private set; }
    public WeatherInfo CurrentWeather { get; private set; }
    public IReadOnlyCollection<WeatherForecast> Forecasts { get; private set; }
    public IReadOnlyCollection<WeatherAlert> Alerts { get; private set; }

#pragma warning disable CS8618
    private LocationWeather() { }
#pragma warning restore

    private LocationWeather(
        Location location, 
        WeatherInfo currentWeather,
        IEnumerable<WeatherForecast> forecasts, 
        IEnumerable<WeatherAlert> alerts)
    {
        Location = location;
        CurrentWeather = currentWeather;
        Forecasts = forecasts?.ToList() ?? [];
        Alerts = alerts?.ToList() ?? [];
    }

    public static LocationWeather Create(
        Location location,
        WeatherInfo currentWeather,
        IEnumerable<WeatherForecast> forecasts,
        IEnumerable<WeatherAlert> alerts)
    {
        if (location is null) throw new ArgumentNullException(nameof(location));

        if(currentWeather is null) throw new ArgumentNullException(nameof(currentWeather));

        if (forecasts.Any(f => f.Date < DateTime.UtcNow.Date))
            throw new InvalidOperationException("Forecast cannot be in the past.");

        if (alerts.Any(a => a.EffectiveFrom > a.EffectiveTo))
            throw new InvalidOperationException("Alert dates are invalid.");

        return new LocationWeather(location, currentWeather, forecasts, alerts);
    }
}
