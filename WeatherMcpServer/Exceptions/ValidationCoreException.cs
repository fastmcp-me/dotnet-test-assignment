namespace WeatherMcpServer.Exceptions;

/// <summary>
/// Represents an exception that occurs during validation.
/// </summary>
/// <param name="errors">The validation errors.</param>
public class ValidationCoreException(Dictionary<string, string[]> errors) : Exception
{
    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public Dictionary<string, string[]> Errors { get; } = errors;
}

