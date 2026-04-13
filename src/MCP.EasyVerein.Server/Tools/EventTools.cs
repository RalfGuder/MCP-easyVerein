using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing events via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class EventTools(IEasyVereinApiClient client)
{
    /// <summary>
    /// Lists events with optional filters and automatic pagination.
    /// </summary>
    /// <param name="name">Optional name filter.</param>
    /// <param name="startGte">Optional start date greater than or equal filter.</param>
    /// <param name="startLte">Optional start date less than or equal filter.</param>
    /// <param name="endGte">Optional end date greater than or equal filter.</param>
    /// <param name="endLte">Optional end date less than or equal filter.</param>
    /// <param name="calendar">Optional calendar ID filter.</param>
    /// <param name="canceled">Optional canceled filter.</param>
    /// <param name="isPublic">Optional public visibility filter.</param>
    /// <param name="idIn">Optional comma-separated IDs filter.</param>
    /// <param name="ordering">Optional ordering criterion.</param>
    /// <param name="search">Optional search terms.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string containing matching events, or an error message.</returns>
    [McpServerTool(Name = "list_events"), Description("List all events")]
    public async Task<string> ListEvents(
        [Description("Filter by event name")] string? name,
        [Description("Start date greater than or equal (ISO 8601)")] string? startGte,
        [Description("Start date less than or equal (ISO 8601)")] string? startLte,
        [Description("End date greater than or equal (ISO 8601)")] string? endGte,
        [Description("End date less than or equal (ISO 8601)")] string? endLte,
        [Description("Filter by calendar ID")] string? calendar,
        [Description("Filter by canceled status (true/false)")] string? canceled,
        [Description("Filter by public visibility (true/false)")] string? isPublic,
        [Description("Filter by comma-separated IDs")] string? idIn,
        [Description("Ordering criteria (e.g. 'start' or '-start')")] string? ordering,
        [Description("Search terms")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var events = await client.ListEventsAsync(name, startGte, startLte, endGte, endLte,
                calendar, canceled, isPublic, idIn, ordering, search, ct);
            return JsonSerializer.Serialize(events, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Retrieves a single event by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the event, or a not-found message.</returns>
    [McpServerTool(Name = "get_event"), Description("Retrieve an event by its ID")]
    public async Task<string> GetEvent(
        [Description("The ID of the event")] long id, CancellationToken ct)
    {
        try
        {
            var ev = await client.GetEventAsync(id, ct);
            return ev != null
                ? JsonSerializer.Serialize(ev, new JsonSerializerOptions { WriteIndented = true })
                : $"Event with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Creates a new event in easyVerein.
    /// </summary>
    /// <param name="name">The name of the event (required).</param>
    /// <param name="description">An optional description.</param>
    /// <param name="locationName">An optional location name.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the created event, or an error message.</returns>
    [McpServerTool(Name = "create_event"), Description("Create a new event")]
    public async Task<string> CreateEvent(
        [Description("The event name (required)")] string name,
        [Description("An optional description")] string? description,
        [Description("An optional location name")] string? locationName,
        CancellationToken ct)
    {
        try
        {
            var ev = new Event { Name = name, Description = description, LocationName = locationName };
            var created = await client.CreateEventAsync(ev, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Updates an existing event. Only the provided fields are modified (PATCH semantics).
    /// </summary>
    /// <param name="id">The unique identifier of the event to update.</param>
    /// <param name="name">Optional new name.</param>
    /// <param name="description">Optional new description.</param>
    /// <param name="locationName">Optional new location name.</param>
    /// <param name="start">Optional new start date (ISO 8601).</param>
    /// <param name="end">Optional new end date (ISO 8601).</param>
    /// <param name="allDay">Optional new all-day flag.</param>
    /// <param name="canceled">Optional new canceled flag.</param>
    /// <param name="isPublic">Optional new public visibility flag.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the updated event, or an error message.</returns>
    [McpServerTool(Name = "update_event"), Description("Update an event (only provided fields are changed)")]
    public async Task<string> UpdateEvent(
        [Description("The ID of the event")] long id,
        [Description("The new name")] string? name,
        [Description("The new description")] string? description,
        [Description("The new location name")] string? locationName,
        [Description("The new start date (ISO 8601)")] string? start,
        [Description("The new end date (ISO 8601)")] string? end,
        [Description("Whether this is an all-day event (true/false)")] string? allDay,
        [Description("Whether the event is canceled (true/false)")] string? canceled,
        [Description("Whether the event is publicly visible (true/false)")] string? isPublic,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (HasValue(name)) patch[EventFields.Name] = name!;
            if (HasValue(description)) patch[EventFields.Description] = description!;
            if (HasValue(locationName)) patch[EventFields.LocationName] = locationName!;
            if (HasValue(start)) patch[EventFields.Start] = start!;
            if (HasValue(end)) patch[EventFields.End] = end!;
            if (HasValue(allDay) && bool.TryParse(allDay, out var allDayVal)) patch[EventFields.AllDay] = allDayVal;
            if (HasValue(canceled) && bool.TryParse(canceled, out var canceledVal)) patch[EventFields.Canceled] = canceledVal;
            if (HasValue(isPublic) && bool.TryParse(isPublic, out var isPublicVal)) patch[EventFields.IsPublic] = isPublicVal;

            return await client.UpdateEventAsync(id, patch, ct);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Deletes an event by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A confirmation message, or an error message.</returns>
    [McpServerTool(Name = "delete_event"), Description("Delete an event. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteEvent(
        [Description("The ID of the event")] long id, CancellationToken ct)
    {
        try
        {
            await client.DeleteEventAsync(id, ct);
            return $"Event with ID {id} has been deleted.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>Checks whether a string parameter has a real value (not null, empty, or the literal "null").</summary>
    private static bool HasValue(string? value) =>
        !string.IsNullOrEmpty(value) && !value.Equals("null", StringComparison.OrdinalIgnoreCase);
}
