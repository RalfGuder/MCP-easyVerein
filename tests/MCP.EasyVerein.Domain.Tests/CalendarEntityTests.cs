using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class CalendarEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 335702286,
                "name": "Kulturverein",
                "color": "#f9e4c6",
                "short": "KVMi",
                "allowedGroups": [{"id": 335646249}],
                "linkedItems": 96,
                "deleteEventsAfterDeletion": false
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var calendar = JsonSerializer.Deserialize<Calendar>(json, options);

        Assert.NotNull(calendar);
        Assert.Equal(335702286L, calendar.Id);
        Assert.Equal("Kulturverein", calendar.Name);
        Assert.Equal("#f9e4c6", calendar.Color);
        Assert.Equal("KVMi", calendar.Short);
        Assert.NotNull(calendar.AllowedGroups);
        Assert.Single(calendar.AllowedGroups);
        Assert.Equal(335646249L, calendar.AllowedGroups[0].Id);
        Assert.Equal(96, calendar.LinkedItems);
        Assert.False(calendar.DeleteEventsAfterDeletion);
    }

    [Fact]
    public void JsonPropertyNames_WithEmptyAllowedGroups_AreCorrect()
    {
        var json = """
            {
                "id": 400324380,
                "name": "Feiertage",
                "color": "#5aaf17",
                "short": "FT",
                "allowedGroups": [],
                "linkedItems": 24,
                "deleteEventsAfterDeletion": false
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var calendar = JsonSerializer.Deserialize<Calendar>(json, options);

        Assert.NotNull(calendar);
        Assert.Equal(400324380L, calendar.Id);
        Assert.Equal("Feiertage", calendar.Name);
        Assert.NotNull(calendar.AllowedGroups);
        Assert.Empty(calendar.AllowedGroups);
    }
}
