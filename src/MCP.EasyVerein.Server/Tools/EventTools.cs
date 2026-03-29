using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

[McpServerToolType]
public sealed class EventTools
{
    private readonly IEasyVereinApiClient _client;

    public EventTools(IEasyVereinApiClient client)
    {
        _client = client;
    }

    [McpServerTool, Description("Alle Veranstaltungen auflisten")]
    public async Task<string> ListEvents(CancellationToken ct)
    {
        var events = await _client.GetEventsAsync(ct);
        return JsonSerializer.Serialize(events, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Eine Veranstaltung anhand der ID abrufen")]
    public async Task<string> GetEvent(long id, CancellationToken ct)
    {
        var ev = await _client.GetEventAsync(id, ct);
        return ev != null
            ? JsonSerializer.Serialize(ev, new JsonSerializerOptions { WriteIndented = true })
            : $"Veranstaltung mit ID {id} nicht gefunden.";
    }

    [McpServerTool, Description("Neue Veranstaltung anlegen")]
    public async Task<string> CreateEvent(string name, string? description, string? location, CancellationToken ct)
    {
        var ev = new Event { Name = name, Description = description, Location = location };
        var created = await _client.CreateEventAsync(ev, ct);
        return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool, Description("Veranstaltung löschen")]
    public async Task<string> DeleteEvent(long id, CancellationToken ct)
    {
        await _client.DeleteEventAsync(id, ct);
        return $"Veranstaltung mit ID {id} wurde gelöscht.";
    }
}
