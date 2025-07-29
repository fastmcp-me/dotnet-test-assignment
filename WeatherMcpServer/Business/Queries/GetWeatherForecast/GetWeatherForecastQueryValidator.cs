using FluentValidation;

namespace WeatherMcpServer.Business.Queries.GetWeatherForecast;

public class GetWeatherForecastQueryValidator : AbstractValidator<GetWeatherForecastQuery>
{
    public GetWeatherForecastQueryValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City name is required.")
            .MinimumLength(2).WithMessage("City name must be at least 2 characters long.")
            .MaximumLength(100).WithMessage("City name must not exceed 100 characters.")
            .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("City name can only contain letters, spaces, hyphens, and apostrophes.");

        RuleFor(x => x.CountryCode)
            .Matches(@"^[A-Z]{2}$").When(x => !string.IsNullOrEmpty(x.CountryCode))
            .WithMessage("Country code must be a 2-letter ISO code (e.g., 'US', 'UK', 'FR').");

        RuleFor(x => x.Days)
            .InclusiveBetween(1, 5).WithMessage("Forecast days must be between 1 and 5.");
    }
}