using FluentValidation;

namespace WeatherMcpServer.Business.Queries.GetCityWeather;

public class GetCityWeatherQueryValidator : AbstractValidator<GetCityWeatherQuery>
{
    public GetCityWeatherQueryValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City name is required.")
            .MinimumLength(2).WithMessage("City name must be at least 2 characters long.")
            .MaximumLength(100).WithMessage("City name must not exceed 100 characters.")
            .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("City name can only contain letters, spaces, hyphens, and apostrophes.");
    }
}