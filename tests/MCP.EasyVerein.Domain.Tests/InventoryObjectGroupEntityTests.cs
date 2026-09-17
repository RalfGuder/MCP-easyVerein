using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class InventoryObjectGroupEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 4711,
                "org": "https://easyverein.com/api/v2.0/organization/30189",
                "_deleteAfterDate": "2026-10-17T10:00:00+02:00",
                "_deletedBy": "Max Mustermann",
                "created_at": "2026-09-17T10:00:00.123456+02:00",
                "updated_at": "2026-09-17T11:00:00.654321+02:00",
                "name": "Zelte",
                "color": "#ff8800",
                "short": "ZLT",
                "linkedItems": ["https://easyverein.com/api/v2.0/inventory-object/335646309"]
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var group = JsonSerializer.Deserialize<InventoryObjectGroup>(json, options);

        Assert.NotNull(group);
        Assert.Equal(4711L, group.Id);
        Assert.Equal("https://easyverein.com/api/v2.0/organization/30189", group.Org);
        Assert.NotNull(group.DeleteAfterDate);
        Assert.Equal("Max Mustermann", group.DeletedBy);
        Assert.NotNull(group.CreatedAt);
        Assert.NotNull(group.UpdatedAt);
        Assert.Equal("Zelte", group.Name);
        Assert.Equal("#ff8800", group.Color);
        Assert.Equal("ZLT", group.Short);
        Assert.NotNull(group.LinkedItems);
        Assert.Equal(JsonValueKind.Array, group.LinkedItems!.Value.ValueKind);
        Assert.Equal(1, group.LinkedItems.Value.GetArrayLength());
    }

    [Fact]
    public void JsonPropertyNames_WithNullValues_AreCorrect()
    {
        var json = """
            {
                "id": 1,
                "_deleteAfterDate": null,
                "name": "Leer",
                "color": "#000000",
                "short": "L"
            }
            """;

        var group = JsonSerializer.Deserialize<InventoryObjectGroup>(json);

        Assert.NotNull(group);
        Assert.Null(group.DeleteAfterDate);
        Assert.Null(group.LinkedItems);
        Assert.Null(group.CreatedAt);
    }

    [Fact]
    public void Serialize_ForCreate_OmitsReadOnlyFields()
    {
        var group = new InventoryObjectGroup { Name = "Zelte", Color = "#ff8800", Short = "ZLT" };

        var json = JsonSerializer.Serialize(group);

        Assert.Contains("\"name\":\"Zelte\"", json);
        Assert.Contains("\"color\":\"#ff8800\"", json);
        Assert.Contains("\"short\":\"ZLT\"", json);
        Assert.DoesNotContain("org", json);
        Assert.DoesNotContain("linkedItems", json);
        Assert.DoesNotContain("created_at", json);
        Assert.DoesNotContain("updated_at", json);
        Assert.DoesNotContain("_delete", json);
    }
}
