using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Helpers;

namespace MCP.EasyVerein.Domain.Converters;

/// <summary>
/// JSON converter for a nullable <see cref="long"/> foreign-key identifier that accepts either a
/// JSON number, a numeric string, or an easyVerein resource URL string
/// (e.g. <c>https://easyverein.com/api/v2.0/contact-details/345175845</c>). The trailing numeric
/// identifier is extracted via <see cref="UrlReference.ExtractId(string?)"/>. Writes a JSON number
/// (the integer id) or JSON null.
/// </summary>
public sealed class FlexibleIdConverter : JsonConverter<long?>
{
    /// <summary>Reads a nullable identifier from a JSON number, numeric string, or resource URL.</summary>
    public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.Number:
                return reader.GetInt64();
            case JsonTokenType.String:
                var s = reader.GetString();
                if (string.IsNullOrWhiteSpace(s)) return null;
                if (long.TryParse(s, out var direct)) return direct;
                return UrlReference.ExtractId(s);
            default:
                return null;
        }
    }

    /// <summary>Writes a nullable identifier as a JSON number, or JSON null.</summary>
    public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else writer.WriteNumberValue(value.Value);
    }
}
