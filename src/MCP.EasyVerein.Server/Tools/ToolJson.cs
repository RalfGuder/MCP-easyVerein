using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// Central JSON serialization for MCP tool responses (US-0064): compact output, no
/// <c>null</c> fields, and references to other resources written as their ID only.
/// Used for tool output only; API request payloads keep their own serializer settings.
/// </summary>
public static class ToolJson
{
    /// <summary>
    /// References that stay embedded because they carry the actual content of the response
    /// rather than a pointer to another resource (the member's name and address, the event location).
    /// </summary>
    private static readonly HashSet<(Type DeclaringType, string PropertyName)> EmbeddedReferences = new()
    {
        (typeof(Member), nameof(Member.ContactDetails)),
        (typeof(Event), nameof(Event.LocationObject)),
    };

    /// <summary>Gets the shared serializer options for all tool responses.</summary>
    public static JsonSerializerOptions Options { get; } = CreateOptions();

    /// <summary>Serializes a tool result with the shared options.</summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to serialize.</param>
    /// <returns>The compact JSON text.</returns>
    /// <exception cref="NotSupportedException">The value contains a type that cannot be serialized.</exception>
    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);

    /// <summary>
    /// Serializes a page of list results as an envelope with <c>items</c>, <c>totalCount</c>,
    /// <c>hasMore</c>, <c>page</c> and <c>limit</c>.
    /// </summary>
    /// <typeparam name="T">The entity type of the list items.</typeparam>
    /// <param name="page">The page to serialize.</param>
    /// <returns>The compact JSON text.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="page"/> is <c>null</c>.</exception>
    /// <exception cref="NotSupportedException">An item contains a type that cannot be serialized.</exception>
    public static string SerializePage<T>(PagedResult<T> page)
    {
        ArgumentNullException.ThrowIfNull(page);
        return Serialize(new PageEnvelope<T>(page.Items, page.TotalCount, page.HasMore, page.Page, page.Limit));
    }

    /// <summary>
    /// Converts the <c>limit</c> and <c>page</c> arguments of a list tool into a page request.
    /// </summary>
    /// <param name="limit">The number of entries per page (1 to <see cref="PageRequest.MaxLimit"/>).</param>
    /// <param name="page">The 1-based page number.</param>
    /// <returns>The page request.</returns>
    /// <exception cref="ArgumentOutOfRangeException">An argument is outside its valid range.</exception>
    public static PageRequest ToPageRequest(int limit, int page) => new(limit, page);

    /// <summary>Creates the shared options with the reference-to-ID modifier.</summary>
    /// <returns>The configured options.</returns>
    private static JsonSerializerOptions CreateOptions()
    {
        var resolver = new DefaultJsonTypeInfoResolver();
        resolver.Modifiers.Add(WriteReferencesAsId);
        return new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = resolver,
        };
    }

    /// <summary>
    /// Replaces the converter of every property that references another resource
    /// (a type implementing <see cref="IHasId"/>, or a list of such) with an ID-only writer.
    /// </summary>
    /// <param name="typeInfo">The type metadata being built.</param>
    private static void WriteReferencesAsId(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }

        foreach (var property in typeInfo.Properties)
        {
            if (IsEmbedded(typeInfo.Type, property))
            {
                continue;
            }

            var converterType = GetIdConverterType(property.PropertyType);
            if (converterType is not null)
            {
                property.CustomConverter = (JsonConverter)Activator.CreateInstance(converterType)!;
            }
        }
    }

    /// <summary>Determines whether a property is one of the references that stay embedded.</summary>
    /// <param name="declaringType">The type that owns the property.</param>
    /// <param name="property">The property metadata.</param>
    /// <returns><c>true</c> if the reference stays embedded.</returns>
    private static bool IsEmbedded(Type declaringType, JsonPropertyInfo property) =>
        property.AttributeProvider is MemberInfo member
        && EmbeddedReferences.Contains((declaringType, member.Name));

    /// <summary>Gets the ID-only converter type for a property type, if it is a reference.</summary>
    /// <param name="propertyType">The declared property type.</param>
    /// <returns>The converter type, or <c>null</c> if the property is no reference.</returns>
    private static Type? GetIdConverterType(Type propertyType)
    {
        if (typeof(IHasId).IsAssignableFrom(propertyType) && propertyType.IsClass)
        {
            return typeof(IdOnlyConverter<>).MakeGenericType(propertyType);
        }

        if (propertyType.IsGenericType
            && propertyType.GetGenericTypeDefinition() == typeof(List<>)
            && typeof(IHasId).IsAssignableFrom(propertyType.GetGenericArguments()[0]))
        {
            return typeof(IdOnlyListConverter<>).MakeGenericType(propertyType.GetGenericArguments()[0]);
        }

        return null;
    }

    /// <summary>The envelope written for a page of list results.</summary>
    /// <typeparam name="T">The entity type of the list items.</typeparam>
    /// <param name="Items">The entries on this page.</param>
    /// <param name="TotalCount">The total number of entries across all pages; omitted if unknown.</param>
    /// <param name="HasMore">Whether further pages exist.</param>
    /// <param name="Page">The 1-based page number.</param>
    /// <param name="Limit">The number of entries per page.</param>
    private sealed record PageEnvelope<T>(
        [property: JsonPropertyName("items")] IReadOnlyList<T> Items,
        [property: JsonPropertyName("totalCount")] int? TotalCount,
        [property: JsonPropertyName("hasMore")] bool HasMore,
        [property: JsonPropertyName("page")] int Page,
        [property: JsonPropertyName("limit")] int Limit);

    /// <summary>Writes a referenced entity as its ID only. Reading is not supported.</summary>
    /// <typeparam name="T">The referenced entity type.</typeparam>
    private sealed class IdOnlyConverter<T> : JsonConverter<T> where T : class, IHasId
    {
        /// <summary>Not supported: tool output is never read back.</summary>
        /// <exception cref="NotSupportedException">Always.</exception>
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            throw new NotSupportedException();

        /// <summary>Writes the ID of the referenced entity.</summary>
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
            writer.WriteNumberValue(value.Id);
    }

    /// <summary>Writes a list of referenced entities as an array of IDs. Reading is not supported.</summary>
    /// <typeparam name="T">The referenced entity type.</typeparam>
    private sealed class IdOnlyListConverter<T> : JsonConverter<List<T>> where T : IHasId
    {
        /// <summary>Not supported: tool output is never read back.</summary>
        /// <exception cref="NotSupportedException">Always.</exception>
        public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            throw new NotSupportedException();

        /// <summary>Writes the IDs of the referenced entities as a JSON array.</summary>
        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
            {
                writer.WriteNumberValue(item.Id);
            }

            writer.WriteEndArray();
        }
    }
}
