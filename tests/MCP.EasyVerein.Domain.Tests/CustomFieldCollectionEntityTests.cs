using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class CustomFieldCollectionEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 777,
                "name": "Vereinsdaten",
                "orderSequence": 2,
                "position": 5
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var collection = JsonSerializer.Deserialize<CustomFieldCollection>(json, options);

        Assert.NotNull(collection);
        Assert.Equal(777L, collection.Id);
        Assert.Equal("Vereinsdaten", collection.Name);
        Assert.Equal(2, collection.OrderSequence);
        Assert.Equal(5, collection.Position);
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
        var collection = JsonSerializer.Deserialize<CustomFieldCollection>(json, options);

        Assert.NotNull(collection);
        Assert.Equal(42L, collection.Id);
        Assert.Equal("Minimal", collection.Name);
        Assert.Null(collection.OrderSequence);
        Assert.Null(collection.Position);
    }
}
