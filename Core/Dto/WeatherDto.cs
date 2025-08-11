namespace Core.Dto;

public record WeatherDto(
	string City,
	double Temperature,
	string WeatherCondition,
	DateTime Timestamp
);