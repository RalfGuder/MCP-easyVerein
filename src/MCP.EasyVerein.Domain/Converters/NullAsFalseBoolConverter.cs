using System.Text.Json;
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Converters;

/// <summary>
/// JSON converter for a non-nullable <see cref="bool"/> flag that reads JSON null as <c>false</c>.
/// easyVerein sometimes returns null for boolean flags (e.g. <c>_isTaxRatePerInvoiceItem</c> on
/// receipts created via the API). Always writes a JSON boolean, so request bodies are unchanged.
/// </summary>
public sealed class NullAsFalseBoolConverter : JsonConverter<bool>
{
    /// <summary>Gets a value indicating that JSON null is passed to <see cref="Read"/>.</summary>
    public override bool HandleNull => true;

    /// <summary>Reads a boolean flag; JSON null yields <c>false</c>.</summary>
    /// <exception cref="JsonException">The token is neither a boolean nor null.</exception>
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => false,
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            _ => throw new JsonException($"Unexpected token {reader.TokenType} for a boolean flag.")
        };
    }

    /// <summary>Writes the flag as a JSON boolean.</summary>
    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}
