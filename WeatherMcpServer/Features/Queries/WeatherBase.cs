using FluentValidation;
using MediatR;

namespace WeatherMcpServer.Features.Queries;

/// <summary>
/// Base record for weather queries.
/// </summary>
/// <param name="City">The city name.</param>
/// <param name="CountryCode">The country code.</param>
public record Query(string City, string? CountryCode);

/// <summary>
/// Base class for weather queries.
/// </summary>
public static class WeatherBase
{
    /// <summary>
    /// Base record for weather queries that implements <see cref="IRequest{TResponse}"/>.
    /// </summary>
    /// <param name="City">The city name.</param>
    /// <param name="CountryCode">The country code.</param>
    public record Query(string City, string? CountryCode): IRequest<string>;

    /// <summary>
    /// Base validator for weather queries.
    /// </summary>
    /// <typeparam name="T">The type of the query.</typeparam>
    internal class Validator<T> : AbstractValidator<T>
        where T : Query
    {
        public Validator()
        {
            RuleFor(x => x.City)
                .NotNull()
                .NotEmpty();
        }
    }
}
