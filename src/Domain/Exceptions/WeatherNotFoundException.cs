namespace Domain.Exceptions;

public class WeatherNotFoundException : Exception
{
    public WeatherNotFoundException(string city)
        : base($"Weather not found for city: {city}") { }

    public WeatherNotFoundException(string city, Exception inner)
        : base($"Weather not found for city: {city}", inner) { }
}