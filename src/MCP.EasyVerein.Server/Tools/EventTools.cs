using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing events via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class EventTools
{
    private readonly IEasyVereinApiClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventTools"/> class.
    /// </summary>
    /// <param name="client">The easyVerein API client.</param>
    public EventTools(IEasyVereinApiClient client) { _client = client; }

    /// <summary>
    /// Lists all events from the easyVerein API with automatic pagination.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string containing all events.</returns>
    [McpServerTool, Description("List all events")]
    public async Task<string> ListEvents(CancellationToken ct)
    {
        var events = await _client.GetEventsAsync(ct);
        return JsonSerializer.Serialize(events, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Retrieves a single event by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the event, or a not-found message.</returns>
    [McpServerTool, Description("Retrieve an event by its ID")]
    public async Task<string> GetEvent(long id, CancellationToken ct)
    {
        var ev = await _client.GetEventAsync(id, ct);
        return ev != null
            ? JsonSerializer.Serialize(ev, new JsonSerializerOptions { WriteIndented = true })
            : $"Event with ID {id} not found.";
    }

    /// <summary>
    /// Creates a new event in easyVerein.
    /// </summary>
    /// <param name="name">The name of the event.</param>
    /// <param name="description">An optional description.</param>
    /// <param name="locationName">An optional location name.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the created event.</returns>
    [McpServerTool, Description("Create a new event")]
    public async Task<string> CreateEvent(
        string name, string? description, string? locationName, CancellationToken ct)
    {
        var ev = new Event { Name = name, Description = description, LocationName = locationName };
        var created = await _client.CreateEventAsync(ev, ct);
        return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Deletes an event by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A confirmation message.</returns>
    [McpServerTool, Description("Delete an event. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteEvent(long id, CancellationToken ct)
    {
        await _client.DeleteEventAsync(id, ct);
        return $"Event with ID {id} has been deleted.";
    }
}
