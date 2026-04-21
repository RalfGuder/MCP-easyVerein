using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class BookingProjectEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 42,
                "name": "Sommerfest 2026",
                "color": "#ff8800",
                "short": "SF26",
                "budget": 1500.75,
                "completed": false,
                "projectCostCentre": "KST-123"
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var project = JsonSerializer.Deserialize<BookingProject>(json, options);

        Assert.NotNull(project);
        Assert.Equal(42L, project.Id);
        Assert.Equal("Sommerfest 2026", project.Name);
        Assert.Equal("#ff8800", project.Color);
        Assert.Equal("SF26", project.Short);
        Assert.Equal(1500.75m, project.Budget);
        Assert.False(project.Completed);
        Assert.Equal("KST-123", project.ProjectCostCentre);
    }

    [Fact]
    public void JsonPropertyNames_WithMinimalPayload_AreCorrect()
    {
        var json = """
            {
                "id": 99,
                "name": "Minimal"
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var project = JsonSerializer.Deserialize<BookingProject>(json, options);

        Assert.NotNull(project);
        Assert.Equal(99L, project.Id);
        Assert.Equal("Minimal", project.Name);
        Assert.Null(project.Color);
        Assert.Null(project.Short);
        Assert.Null(project.Budget);
        Assert.Null(project.Completed);
        Assert.Null(project.ProjectCostCentre);
    }
}
