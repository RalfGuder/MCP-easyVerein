using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;

namespace MCP.EasyVerein.Domain.Tests;

public class ChairmanLevelEntityTests
{
    [Fact]
    public void JsonPropertyNames_AreCorrect()
    {
        var json = """
            {
                "id": 7,
                "name": "Vorstand",
                "color": "#336699",
                "short": "VS",
                "module_members": "W",
                "module_events": "W",
                "module_protocols": "R",
                "module_addresses": "W",
                "module_bookings": "N",
                "module_inventory": "R",
                "module_files": "W",
                "module_account": "N",
                "module_todo": "W",
                "module_votings": "R",
                "module_forum": "W"
            }
            """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var level = JsonSerializer.Deserialize<ChairmanLevel>(json, options);

        Assert.NotNull(level);
        Assert.Equal(7L, level.Id);
        Assert.Equal("Vorstand", level.Name);
        Assert.Equal("#336699", level.Color);
        Assert.Equal("VS", level.Short);
        Assert.Equal("W", level.ModuleMembers);
        Assert.Equal("W", level.ModuleEvents);
        Assert.Equal("R", level.ModuleProtocols);
        Assert.Equal("W", level.ModuleAddresses);
        Assert.Equal("N", level.ModuleBookings);
        Assert.Equal("R", level.ModuleInventory);
        Assert.Equal("W", level.ModuleFiles);
        Assert.Equal("N", level.ModuleAccount);
        Assert.Equal("W", level.ModuleTodo);
        Assert.Equal("R", level.ModuleVotings);
        Assert.Equal("W", level.ModuleForum);
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
        var level = JsonSerializer.Deserialize<ChairmanLevel>(json, options);

        Assert.NotNull(level);
        Assert.Equal(99L, level.Id);
        Assert.Equal("Minimal", level.Name);
        Assert.Null(level.Color);
        Assert.Null(level.Short);
        Assert.Null(level.ModuleMembers);
        Assert.Null(level.ModuleEvents);
        Assert.Null(level.ModuleProtocols);
        Assert.Null(level.ModuleAddresses);
        Assert.Null(level.ModuleBookings);
        Assert.Null(level.ModuleInventory);
        Assert.Null(level.ModuleFiles);
        Assert.Null(level.ModuleAccount);
        Assert.Null(level.ModuleTodo);
        Assert.Null(level.ModuleVotings);
        Assert.Null(level.ModuleForum);
    }
}
