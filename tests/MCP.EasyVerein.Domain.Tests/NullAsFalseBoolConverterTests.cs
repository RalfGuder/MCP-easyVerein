using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;

namespace MCP.EasyVerein.Domain.Tests;

/// <summary>Unit tests for <see cref="NullAsFalseBoolConverter"/>.</summary>
public class NullAsFalseBoolConverterTests
{
    /// <summary>Test holder that applies the converter to a single boolean property.</summary>
    private sealed class Holder
    {
        /// <summary>Gets or sets the converted flag.</summary>
        [JsonConverter(typeof(NullAsFalseBoolConverter))]
        public bool Value { get; set; }
    }

    /// <summary>Verifies that JSON null is read as <c>false</c> instead of throwing.</summary>
    [Fact]
    public void Read_FromNull_ReturnsFalse()
    {
        var holder = JsonSerializer.Deserialize<Holder>("""{ "Value": null }""");

        Assert.False(holder!.Value);
    }

    /// <summary>Verifies that JSON booleans are read unchanged.</summary>
    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void Read_FromBoolean_ReturnsValue(string json, bool expected)
    {
        var holder = JsonSerializer.Deserialize<Holder>($$"""{ "Value": {{json}} }""");

        Assert.Equal(expected, holder!.Value);
    }

    /// <summary>Verifies that the flag is always written as a JSON boolean.</summary>
    [Theory]
    [InlineData(true, """{"Value":true}""")]
    [InlineData(false, """{"Value":false}""")]
    public void Write_WritesJsonBoolean(bool value, string expected)
    {
        var json = JsonSerializer.Serialize(new Holder { Value = value });

        Assert.Equal(expected, json);
    }
}
