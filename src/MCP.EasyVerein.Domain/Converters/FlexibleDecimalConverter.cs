using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Converters;

/// <summary>
/// JSON converter for nullable <see cref="decimal"/> that accepts either a numeric token
/// (easyVerein API v1.7 monetary fields) or a string-encoded number (easyVerein API v2.0).
/// </summary>
public sealed class FlexibleDecimalConverter : JsonConverter<decimal?>
{
    /// <summary>Reads a nullable <see cref="decimal"/> accepting Number or String tokens.</summary>
    public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.Number:
                return reader.GetDecimal();
            case JsonTokenType.String:
                var s = reader.GetString();
                if (string.IsNullOrWhiteSpace(s)) return null;
                return decimal.Parse(s, NumberStyles.Number, CultureInfo.InvariantCulture);
            default:
                throw new JsonException($"Unexpected token {reader.TokenType} when parsing decimal.");
        }
    }

    /// <summary>Writes a nullable <see cref="decimal"/> as JSON number or null.</summary>
    public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else writer.WriteNumberValue(value.Value);
    }
}
