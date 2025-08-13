namespace WeatherMcpServer.Domain.LocationAggregate;

public record Location
{
    public string City { get; }
    public string? CountryCode { get; }

    private Location(string city, string? countryCode = null)
    {
        City = city.Trim();
        CountryCode = countryCode?.ToUpperInvariant();
    }

    public static Location Create(string city, string? countryCode = null)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City name cannot be empty", nameof(city));

        if (countryCode is not null && (countryCode.Length != 2 || !countryCode.All(char.IsLetter)))
            throw new ArgumentException("Country code must be ISO 3166-1 alpha-2 format", nameof(countryCode));

        return new Location(city, countryCode);
    }

    public override string ToString() =>
        CountryCode is null ? City : $"{City},{CountryCode}";
}