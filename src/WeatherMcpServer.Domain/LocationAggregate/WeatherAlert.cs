namespace WeatherMcpServer.Domain.LocationAggregate;


public enum WeatherAlertSeverity
{
    Advisory,
    Watch,
    Warning
}

public class WeatherAlert
{
    public string Title { get; }
    public string Description { get; }
    public WeatherAlertSeverity Severity { get; }
    public DateTime EffectiveFrom { get; }
    public DateTime EffectiveTo { get; }

    public WeatherAlert(
        string title,
        string description,
        WeatherAlertSeverity severity,
        DateTime effectiveFrom,
        DateTime effectiveTo)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        if (effectiveFrom > effectiveTo)
            throw new ArgumentException("EffectiveFrom cannot be after EffectiveTo");

        Title = title;
        Description = description;
        Severity = severity;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
    }
}