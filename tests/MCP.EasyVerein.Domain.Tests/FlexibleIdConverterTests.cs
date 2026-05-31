using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;

namespace MCP.EasyVerein.Domain.Tests;

public class FlexibleIdConverterTests
{
    private sealed class Holder
    {
        [JsonConverter(typeof(FlexibleIdConverter))]
        public long? Value { get; set; }
    }

    [Fact]
    public void Read_FromJsonNumber_ReturnsId()
    {
        var holder = JsonSerializer.Deserialize<Holder>("""{ "Value": 12345 }""");
        Assert.NotNull(holder);
        Assert.Equal(12345L, holder.Value);
    }

    [Fact]
    public void Read_FromUrlString_ExtractsTrailingId()
    {
        var holder = JsonSerializer.Deserialize<Holder>(
            """{ "Value": "https://easyverein.com/api/v2.0/contact-details/345175845" }""");
        Assert.NotNull(holder);
        Assert.Equal(345175845L, holder.Value);
    }

    [Fact]
    public void Read_FromNumericString_ReturnsId()
    {
        var holder = JsonSerializer.Deserialize<Holder>("""{ "Value": "678" }""");
        Assert.NotNull(holder);
        Assert.Equal(678L, holder.Value);
    }

    [Fact]
    public void Read_FromNull_ReturnsNull()
    {
        var holder = JsonSerializer.Deserialize<Holder>("""{ "Value": null }""");
        Assert.NotNull(holder);
        Assert.Null(holder.Value);
    }

    [Fact]
    public void Read_FromEmptyString_ReturnsNull()
    {
        var holder = JsonSerializer.Deserialize<Holder>("""{ "Value": "" }""");
        Assert.NotNull(holder);
        Assert.Null(holder.Value);
    }

    [Fact]
    public void Write_EmitsJsonNumber()
    {
        var json = JsonSerializer.Serialize(new Holder { Value = 99L });
        Assert.Contains("\"Value\":99", json);
    }
}
