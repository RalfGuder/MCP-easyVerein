using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Helpers;
using MCP.EasyVerein.Domain.Interfaces;

namespace MCP.EasyVerein.Domain.Converters;

/// <summary>
/// Generic JSON converter for an entity reference that may arrive as an embedded object
/// (v1.7 and most v2.0 GET responses) or as a URL-reference string (v2.0 PATCH/list
/// responses). For URL references, only <see cref="IHasId.Id"/> is populated; other
/// properties remain at their defaults. Writes emit an embedded object.
/// </summary>
/// <typeparam name="T">The referenced entity type — must be a reference type with a parameterless constructor implementing <see cref="IHasId"/>.</typeparam>
public sealed class FlexibleReferenceConverter<T> : JsonConverter<T?> where T : class, IHasId, new()
{
    /// <summary>Reads a <typeparamref name="T"/> from either an embedded object or a URL string.</summary>
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.StartObject:
                return JsonSerializer.Deserialize<T>(ref reader, options);
            case JsonTokenType.String:
                var url = reader.GetString();
                var id = UrlReference.ExtractId(url);
                if (id is null)
                    throw new JsonException($"Cannot extract {typeof(T).Name} id from URL: '{url}'.");
                return new T { Id = id.Value };
            default:
                throw new JsonException($"Unexpected token {reader.TokenType} for {typeof(T).Name} reference.");
        }
    }

    /// <summary>Writes a <typeparamref name="T"/> as an embedded JSON object or JSON null.</summary>
    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else JsonSerializer.Serialize(writer, value, options);
    }
}
