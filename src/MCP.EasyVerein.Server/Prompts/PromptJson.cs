using System.Text.Json;
using System.Text.Json.Serialization;
using ModelContextProtocol;

namespace MCP.EasyVerein.Server.Prompts;

/// <summary>
/// JSON serializer options for MCP prompt argument binding. The MCP protocol transmits
/// prompt arguments as strings (e.g. <c>"dryRun": "false"</c>), so boolean parameters
/// must accept both JSON booleans and the strings "true"/"false".
/// </summary>
public static class PromptJson
{
    /// <summary>Serializer options based on the MCP defaults plus a lenient boolean converter.</summary>
    public static JsonSerializerOptions SerializerOptions { get; } = CreateOptions();

    /// <summary>Creates the serializer options used by <see cref="SerializerOptions"/>.</summary>
    /// <returns>A read-only options instance.</returns>
    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions(McpJsonUtilities.DefaultOptions);
        options.Converters.Insert(0, new LenientBooleanConverter());
        options.MakeReadOnly();
        return options;
    }

    /// <summary>Reads booleans from JSON booleans or from the strings "true"/"false" (case-insensitive).</summary>
    private sealed class LenientBooleanConverter : JsonConverter<bool>
    {
        /// <summary>Reads a boolean value from a JSON boolean or string token.</summary>
        /// <param name="reader">The JSON reader.</param>
        /// <param name="typeToConvert">The target type.</param>
        /// <param name="options">The serializer options.</param>
        /// <returns>The parsed boolean.</returns>
        /// <exception cref="JsonException">Thrown for values that are not a boolean.</exception>
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.True: return true;
                case JsonTokenType.False: return false;
                case JsonTokenType.String when bool.TryParse(reader.GetString()?.Trim(), out var value): return value;
                default: throw new JsonException("Expected a boolean value (true or false).");
            }
        }

        /// <summary>Writes the boolean as JSON boolean.</summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="options">The serializer options.</param>
        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
            => writer.WriteBooleanValue(value);
    }
}
