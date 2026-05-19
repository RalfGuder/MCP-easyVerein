using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class ContactDetailsGroupEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 11,
                "name": "Newsletter",
                "color": "#aabbcc",
                "short": "NL",
                "orderSequence": 3
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var group = JsonSerializer.Deserialize<ContactDetailsGroup>(json, options);

        Assert.NotNull(group);
        Assert.Equal(11L, group.Id);
        Assert.Equal("Newsletter", group.Name);
        Assert.Equal("#aabbcc", group.Color);
        Assert.Equal("NL", group.Short);
        Assert.Equal(3, group.OrderSequence);
    }

    [Fact]
    public void JsonPropertyNames_WithMinimalPayload_AreCorrect()
    {
        var json = """
            {
                "id": 42,
                "name": "Minimal"
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var group = JsonSerializer.Deserialize<ContactDetailsGroup>(json, options);

        Assert.NotNull(group);
        Assert.Equal(42L, group.Id);
        Assert.Equal("Minimal", group.Name);
        Assert.Null(group.Color);
        Assert.Null(group.Short);
        Assert.Null(group.OrderSequence);
    }
}
