using System.Text.Json;
using MCP.EasyVerein.Domain.Converters;

namespace MCP.EasyVerein.Domain.Tests;

public class FlexibleDateTimeConverterTests
{
    private sealed class Probe
    {
        [System.Text.Json.Serialization.JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime? Value { get; set; }
    }

    [Theory]
    [InlineData("\"2025-03-26\"", 2025, 3, 26)]
    [InlineData("\"2025-03-26T00:00:00\"", 2025, 3, 26)]
    [InlineData("\"2025-03-26T12:34:56Z\"", 2025, 3, 26)]
    public void Read_ParsesDateOnlyAndIsoDateTime(string json, int y, int m, int d)
    {
        var probe = JsonSerializer.Deserialize<Probe>($"{{\"Value\":{json}}}");
        Assert.NotNull(probe);
        Assert.Equal(new DateTime(y, m, d), probe!.Value!.Value.Date);
    }

    [Fact]
    public void Read_ReturnsNullForJsonNull()
    {
        var probe = JsonSerializer.Deserialize<Probe>("{\"Value\":null}");
        Assert.Null(probe!.Value);
    }

    [Fact]
    public void Write_EmitsIsoRoundTrip()
    {
        var probe = new Probe { Value = new DateTime(2025, 3, 26) };
        var json = JsonSerializer.Serialize(probe);
        Assert.Contains("\"2025-03-26", json);
    }
}
