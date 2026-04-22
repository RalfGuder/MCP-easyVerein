using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Helpers;

namespace MCP.EasyVerein.Domain.Converters;

/// <summary>
/// JSON converter for a list of <see cref="Invoice"/> that accepts either a full embedded
/// object (easyVerein API v1.7 and GET responses in v2.0) or a URL-reference string (v2.0
/// PATCH responses). URL references populate only <see cref="Invoice.Id"/>.
/// </summary>
public sealed class FlexibleInvoiceListConverter : JsonConverter<List<Invoice>?>
{
    /// <summary>Reads a list of <see cref="Invoice"/> from a JSON array whose elements may be objects or URL strings.</summary>
    public override List<Invoice>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException($"Expected StartArray for relatedInvoice, got {reader.TokenType}.");

        var list = new List<Invoice>();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    var invoice = JsonSerializer.Deserialize<Invoice>(ref reader, options);
                    if (invoice != null) list.Add(invoice);
                    break;
                case JsonTokenType.String:
                    var url = reader.GetString();
                    var id = UrlReference.ExtractId(url);
                    if (id is null)
                        throw new JsonException($"Cannot extract Invoice id from URL: '{url}'.");
                    list.Add(new Invoice { Id = id.Value });
                    break;
                case JsonTokenType.Null:
                    // skip explicit nulls inside the array
                    break;
                default:
                    throw new JsonException($"Unexpected token {reader.TokenType} inside relatedInvoice array.");
            }
        }
        return list;
    }

    /// <summary>Writes a list of <see cref="Invoice"/> as a JSON array of embedded objects.</summary>
    public override void Write(Utf8JsonWriter writer, List<Invoice>? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }
        writer.WriteStartArray();
        foreach (var invoice in value)
            JsonSerializer.Serialize(writer, invoice, options);
        writer.WriteEndArray();
    }
}
