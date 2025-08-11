using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core;

internal class AlertLevelJsonConverter : JsonConverter<AlertLevel>
{
	public override AlertLevel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int value))
		{
			return AlertLevel.FromValue(value);
		}
		return null;
	}

	public override void Write(Utf8JsonWriter writer, AlertLevel value, JsonSerializerOptions options)
	{
		writer.WriteNumberValue(value.Value);
	}
}
