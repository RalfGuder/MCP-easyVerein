using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Converters;

/// <summary>
/// JSON converter for nullable <see cref="DateTime"/> that accepts either a date-only
/// string (<c>yyyy-MM-dd</c>) as returned by the easyVerein API for plain date fields,
/// or a full ISO-8601 datetime.
/// </summary>
public sealed class FlexibleDateTimeConverter : JsonConverter<DateTime?>
{
    /// <summary>Reads a nullable <see cref="DateTime"/> accepting date-only or ISO datetimes.</summary>
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        var s = reader.GetString();
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal, out var date))
            return date;
        return DateTime.Parse(s!, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }

    /// <summary>Writes a nullable <see cref="DateTime"/> as ISO-8601 or JSON null.</summary>
    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else writer.WriteStringValue(value.Value.ToString("O", CultureInfo.InvariantCulture));
    }
}
