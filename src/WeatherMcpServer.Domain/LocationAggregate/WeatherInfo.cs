namespace WeatherMcpServer.Domain.LocationAggregate;

public class WeatherInfo
{
    public string Description { get; }
    public decimal TemperatureCelsius { get; }
    public decimal FeelsLikeCelsius { get; }
    public int Humidity { get; }

    private WeatherInfo(string description, decimal temperatureCelsius, decimal feelsLikeCelsius, int humidity)
    {
        Description = description;
        TemperatureCelsius = temperatureCelsius;
        FeelsLikeCelsius = feelsLikeCelsius;
        Humidity = humidity;
    }

    public static WeatherInfo Create(string description, decimal temperatureCelsius, decimal feelsLikeCelsius, int humidity)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        if (temperatureCelsius < -100 || temperatureCelsius > 100)
            throw new ArgumentOutOfRangeException(nameof(temperatureCelsius), "Temperature must be realistic");

        if (feelsLikeCelsius < -100 || feelsLikeCelsius > 100)
            throw new ArgumentOutOfRangeException(nameof(feelsLikeCelsius), "Feels like temperature must be realistic");

        if (humidity < 0 || humidity > 100)
            throw new ArgumentOutOfRangeException(nameof(humidity), "Humidity must be between 0 and 100");

        return new WeatherInfo(description, temperatureCelsius, feelsLikeCelsius, humidity);
    }
}