using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Helpers;
using MCP.EasyVerein.Domain.Interfaces;

namespace MCP.EasyVerein.Domain.Converters;

/// <summary>
/// Generic JSON converter for a list of entity references whose elements may be embedded
/// objects or URL-reference strings — mirrors <see cref="FlexibleReferenceConverter{T}"/>
/// but operates on <see cref="List{T}"/>. Elements of different shapes can be mixed; null
/// elements inside the array are skipped.
/// </summary>
/// <typeparam name="T">The referenced entity type — must implement <see cref="IHasId"/>.</typeparam>
public sealed class FlexibleReferenceListConverter<T> : JsonConverter<List<T>?> where T : class, IHasId, new()
{
    /// <summary>Reads a list of <typeparamref name="T"/> from a JSON array with mixed element shapes.</summary>
    public override List<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException($"Expected StartArray for list of {typeof(T).Name}, got {reader.TokenType}.");

        var list = new List<T>();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    var item = JsonSerializer.Deserialize<T>(ref reader, options);
                    if (item != null) list.Add(item);
                    break;
                case JsonTokenType.String:
                    var url = reader.GetString();
                    var id = UrlReference.ExtractId(url);
                    if (id is null)
                        throw new JsonException($"Cannot extract {typeof(T).Name} id from URL: '{url}'.");
                    list.Add(new T { Id = id.Value });
                    break;
                case JsonTokenType.Null:
                    // skip explicit nulls inside the array
                    break;
                default:
                    throw new JsonException($"Unexpected token {reader.TokenType} inside list of {typeof(T).Name}.");
            }
        }
        return list;
    }

    /// <summary>Writes a list of <typeparamref name="T"/> as a JSON array of embedded objects.</summary>
    public override void Write(Utf8JsonWriter writer, List<T>? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }
        writer.WriteStartArray();
        foreach (var item in value)
            JsonSerializer.Serialize(writer, item, options);
        writer.WriteEndArray();
    }
}
