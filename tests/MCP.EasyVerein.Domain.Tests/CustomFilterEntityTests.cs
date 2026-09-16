using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class CustomFilterEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 73383,
                "name": "2024",
                "model": "bookingFilter",
                "rules": {
                    "condition": "AND",
                    "rules": [
                        {"id": "date", "field": "date", "type": "date", "operator": "greater_or_equal", "value": "2024-01-01"}
                    ],
                    "valid": true
                },
                "created_at": "2026-05-27T06:23:55.206540+02:00",
                "updated_at": "2026-05-27T06:23:55.308413+02:00"
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var filter = JsonSerializer.Deserialize<CustomFilter>(json, options);

        Assert.NotNull(filter);
        Assert.Equal(73383L, filter.Id);
        Assert.Equal("2024", filter.Name);
        Assert.Equal("bookingFilter", filter.Model);
        Assert.NotNull(filter.Rules);
        Assert.Equal("AND", filter.Rules!.Value.GetProperty("condition").GetString());
        Assert.Equal(1, filter.Rules.Value.GetProperty("rules").GetArrayLength());
        Assert.Equal(new DateTimeOffset(2026, 5, 27, 6, 23, 55, 206, TimeSpan.FromHours(2)).AddTicks(5400), filter.CreatedAt);
        Assert.NotNull(filter.UpdatedAt);
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
        var filter = JsonSerializer.Deserialize<CustomFilter>(json, options);

        Assert.NotNull(filter);
        Assert.Equal(42L, filter.Id);
        Assert.Equal("Minimal", filter.Name);
        Assert.Null(filter.Model);
        Assert.Null(filter.Rules);
        Assert.Null(filter.CreatedAt);
        Assert.Null(filter.UpdatedAt);
    }

    [Fact]
    public void Serialize_WritesRulesAsJsonObject()
    {
        using var doc = JsonDocument.Parse("""{"condition":"OR","rules":[]}""");
        var filter = new CustomFilter { Name = "X", Model = "userFilter", Rules = doc.RootElement.Clone() };

        var json = JsonSerializer.Serialize(filter);

        Assert.Contains("\"rules\":{\"condition\":\"OR\",\"rules\":[]}", json);
    }
}
