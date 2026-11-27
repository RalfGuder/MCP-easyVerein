using System.Text.Json;
using MCP.EasyVerein.Server.Prompts;

namespace MCP.EasyVerein.Server.Tests;

/// <summary>Tests for <see cref="PromptJson"/> (prompt arguments arrive as strings in MCP).</summary>
public class PromptJsonTests
{
    /// <summary>Booleans are accepted as JSON booleans and as strings.</summary>
    [Theory]
    [InlineData("false", false)]
    [InlineData("true", true)]
    [InlineData("\"false\"", false)]
    [InlineData("\"True\"", true)]
    [InlineData("\" false \"", false)]
    public void Bool_AusBooleanOderString(string json, bool erwartet)
        => Assert.Equal(erwartet, JsonSerializer.Deserialize<bool>(json, PromptJson.SerializerOptions));

    /// <summary>Non-boolean strings are rejected.</summary>
    [Theory]
    [InlineData("\"nein\"")]
    [InlineData("1")]
    public void Bool_Ungueltig_WirftJsonException(string json)
        => Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<bool>(json, PromptJson.SerializerOptions));

    /// <summary>Strings are still deserialized normally.</summary>
    [Fact]
    public void String_BleibtString()
        => Assert.Equal("2024-12-31", JsonSerializer.Deserialize<string>("\"2024-12-31\"", PromptJson.SerializerOptions));
}
