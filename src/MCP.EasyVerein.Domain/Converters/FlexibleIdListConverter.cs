using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Converters;

/// <summary>
/// JSON converter for a list of foreign-key identifiers whose elements may be JSON numbers,
/// numeric strings, or easyVerein resource URL strings — the list counterpart of
/// <see cref="FlexibleIdConverter"/>. Writes a JSON array of plain integer IDs, as the API
/// expects them on write.
/// </summary>
public sealed class FlexibleIdListConverter : JsonConverter<List<long>?>
{
    /// <summary>The element converter that extracts a single identifier.</summary>
    private static readonly FlexibleIdConverter ElementConverter = new();

    /// <summary>Reads a list of identifiers from a JSON array with mixed element shapes.</summary>
    public override List<long>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException($"Expected StartArray for a list of IDs, got {reader.TokenType}.");

        var list = new List<long>();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            var id = ElementConverter.Read(ref reader, typeof(long?), options)
                ?? throw new JsonException($"Cannot extract an ID from list element of type {reader.TokenType}.");
            list.Add(id);
        }
        return list;
    }

    /// <summary>Writes a list of identifiers as a JSON array of integers, or JSON null.</summary>
    public override void Write(Utf8JsonWriter writer, List<long>? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }
        writer.WriteStartArray();
        foreach (var id in value)
            writer.WriteNumberValue(id);
        writer.WriteEndArray();
    }
}
