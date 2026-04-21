using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;

namespace MCP.EasyVerein.Domain.Tests;

public class FlexibleDecimalConverterTests
{
    private sealed class Wrapper
    {
        [JsonConverter(typeof(FlexibleDecimalConverter))]
        public decimal? Value { get; set; }
    }

    [Fact]
    public void Read_NumberToken_ReturnsDecimal()
    {
        var w = JsonSerializer.Deserialize<Wrapper>("{\"Value\":0.00}");
        Assert.Equal(0.00m, w!.Value);
    }

    [Fact]
    public void Read_StringNumberToken_ReturnsDecimal()
    {
        var w = JsonSerializer.Deserialize<Wrapper>("{\"Value\":\"0.00\"}");
        Assert.Equal(0.00m, w!.Value);
    }

    [Fact]
    public void Read_StringNumberTokenWithFraction_ReturnsDecimal()
    {
        var w = JsonSerializer.Deserialize<Wrapper>("{\"Value\":\"12.34\"}");
        Assert.Equal(12.34m, w!.Value);
    }

    [Fact]
    public void Read_NullToken_ReturnsNull()
    {
        var w = JsonSerializer.Deserialize<Wrapper>("{\"Value\":null}");
        Assert.Null(w!.Value);
    }

    [Fact]
    public void Read_EmptyString_ReturnsNull()
    {
        var w = JsonSerializer.Deserialize<Wrapper>("{\"Value\":\"\"}");
        Assert.Null(w!.Value);
    }

    [Fact]
    public void Read_BooleanToken_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<Wrapper>("{\"Value\":true}"));
    }

    [Fact]
    public void Write_NullValue_WritesJsonNull()
    {
        var json = JsonSerializer.Serialize(new Wrapper { Value = null });
        Assert.Equal("{\"Value\":null}", json);
    }

    [Fact]
    public void Write_DecimalValue_WritesJsonNumber()
    {
        var json = JsonSerializer.Serialize(new Wrapper { Value = 1.23m });
        Assert.Equal("{\"Value\":1.23}", json);
    }
}
