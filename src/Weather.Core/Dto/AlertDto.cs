using System.Text.Json.Serialization;

namespace Weather.Core.Dto;

public record AlertDto(
	double Temperature,
	string Event,
	[property: JsonConverter(typeof(AlertLevelJsonConverter))]
	AlertLevel AlertLevel
);
