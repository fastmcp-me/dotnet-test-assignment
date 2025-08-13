namespace WeatherMcpServer.Domain.LocationAggregate;

public class WeatherForecast
{
    public DateTime Date { get; }
    public string Description { get; }
    public decimal TemperatureCelsius { get; }

    private WeatherForecast(DateTime date, string description, decimal temperatureCelsius)
    {
        Date = date;
        Description = description;
        TemperatureCelsius = temperatureCelsius;
    }

    public static WeatherForecast Create(DateTime date, string description, decimal temperatureCelsius)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        if (temperatureCelsius < -100 || temperatureCelsius > 100)
            throw new ArgumentOutOfRangeException(nameof(temperatureCelsius), "Temperature must be realistic");

        return new WeatherForecast(date, description, temperatureCelsius);
    }
}