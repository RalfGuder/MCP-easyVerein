using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Helpers;

namespace MCP.EasyVerein.Domain.Converters;

/// <summary>
/// JSON converter for <see cref="ContactDetails"/> on <see cref="Member"/> that accepts either
/// a full embedded object (easyVerein API v1.7) or a URL-reference string (easyVerein API v2.0).
/// For URL references, only <see cref="ContactDetails.Id"/> is populated; other properties
/// remain at their default values.
/// </summary>
public sealed class MemberContactDetailsConverter : JsonConverter<ContactDetails?>
{
    /// <summary>Reads a <see cref="ContactDetails"/> from an embedded object or a URL reference.</summary>
    public override ContactDetails? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.StartObject:
                return JsonSerializer.Deserialize<ContactDetails>(ref reader, options);
            case JsonTokenType.String:
                var url = reader.GetString();
                var id = UrlReference.ExtractId(url);
                if (id is null)
                    throw new JsonException($"Cannot extract ContactDetails id from URL: '{url}'.");
                return new ContactDetails { Id = id.Value };
            default:
                throw new JsonException($"Unexpected token {reader.TokenType} for contactDetails.");
        }
    }

    /// <summary>Writes a <see cref="ContactDetails"/> as an embedded object or JSON null.</summary>
    public override void Write(Utf8JsonWriter writer, ContactDetails? value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else JsonSerializer.Serialize(writer, value, options);
    }
}
