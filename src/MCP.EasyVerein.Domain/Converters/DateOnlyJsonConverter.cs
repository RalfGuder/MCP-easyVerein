using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Converters;

/// <summary>
/// JSON converter for nullable <see cref="DateTime"/> mapped to an easyVerein field of
/// type <c>date</c>. It reads date-only strings (<c>yyyy-MM-dd</c>) as well as full
/// ISO-8601 datetimes, but always writes the date-only form the API expects.
/// </summary>
public sealed class DateOnlyJsonConverter : JsonConverter<DateTime?>
{
    /// <summary>The date-only format used by the easyVerein API for <c>date</c> fields.</summary>
    private const string DateFormat = "yyyy-MM-dd";

    /// <summary>Reads a nullable <see cref="DateTime"/> accepting date-only or ISO datetimes.</summary>
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        var s = reader.GetString();
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (DateTime.TryParseExact(s, DateFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal, out var date))
            return date;
        return DateTime.Parse(s!, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }

    /// <summary>Writes a nullable <see cref="DateTime"/> as a date-only string or JSON null.</summary>
    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else writer.WriteStringValue(value.Value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}
