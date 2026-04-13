using System.ComponentModel;
using System.Text.Json;
using MCP.EasyVerein.Domain.Entities;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using ModelContextProtocol.Server;

namespace MCP.EasyVerein.Server.Tools;

/// <summary>
/// MCP tools for managing calendars via the easyVerein API.
/// </summary>
[McpServerToolType]
public sealed class CalendarTools(IEasyVereinApiClient client)
{
    /// <summary>
    /// Lists calendars with optional filters and automatic pagination.
    /// </summary>
    /// <param name="name">Optional name filter.</param>
    /// <param name="color">Optional color filter.</param>
    /// <param name="short_">Optional short name filter.</param>
    /// <param name="nameNot">Optional name negation filter.</param>
    /// <param name="colorNot">Optional color negation filter.</param>
    /// <param name="shortNot">Optional short name negation filter.</param>
    /// <param name="idIn">Optional comma-separated IDs filter.</param>
    /// <param name="allowedGroups">Optional allowed groups filter.</param>
    /// <param name="ordering">Optional ordering criterion.</param>
    /// <param name="search">Optional search terms.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string containing matching calendars, or an error message.</returns>
    [McpServerTool(Name = "list_calendars"), Description("List all calendars")]
    public async Task<string> ListCalendars(
        [Description("Filter by calendar name")] string? name,
        [Description("Filter by color (hex value)")] string? color,
        [Description("Filter by short name")] string? short_,
        [Description("Exclude calendars with this name")] string? nameNot,
        [Description("Exclude calendars with this color")] string? colorNot,
        [Description("Exclude calendars with this short name")] string? shortNot,
        [Description("Filter by comma-separated IDs")] string? idIn,
        [Description("Filter by allowed group ID")] string? allowedGroups,
        [Description("Ordering criteria (e.g. 'name' or '-name')")] string? ordering,
        [Description("Search terms")] string[]? search,
        CancellationToken ct)
    {
        try
        {
            var calendars = await client.ListCalendarsAsync(name, color, short_, nameNot, colorNot,
                shortNot, idIn, allowedGroups, ordering, search, ct);
            return JsonSerializer.Serialize(calendars, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Retrieves a single calendar by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the calendar.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the calendar, or a not-found message.</returns>
    [McpServerTool(Name = "get_calendar"), Description("Retrieve a calendar by its ID")]
    public async Task<string> GetCalendar(
        [Description("The ID of the calendar")] long id, CancellationToken ct)
    {
        try
        {
            var calendar = await client.GetCalendarAsync(id, ct);
            return calendar != null
                ? JsonSerializer.Serialize(calendar, new JsonSerializerOptions { WriteIndented = true })
                : $"Calendar with ID {id} not found.";
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Creates a new calendar in easyVerein.
    /// </summary>
    /// <param name="name">The calendar name (required, max 200 characters).</param>
    /// <param name="color">Optional hex color value (max 7 characters, e.g. '#FF5733').</param>
    /// <param name="short_">Optional short name/abbreviation (max 4 characters, must be unique).</param>
    /// <param name="allowedGroupIds">Optional array of member group IDs that have access.</param>
    /// <param name="deleteEventsAfterDeletion">Optional flag whether to delete events when calendar is deleted.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the created calendar, or an error message.</returns>
    [McpServerTool(Name = "create_calendar"), Description("Create a new calendar")]
    public async Task<string> CreateCalendar(
        [Description("The calendar name (required)")] string name,
        [Description("Hex color value (e.g. '#FF5733')")] string? color,
        [Description("Short name / abbreviation (max 4 chars, must be unique)")] string? short_,
        [Description("Array of member group IDs with access")] long[]? allowedGroupIds,
        [Description("Delete events when calendar is deleted (true/false, default: false)")] string? deleteEventsAfterDeletion,
        CancellationToken ct)
    {
        try
        {
            bool? deleteFlag = null;
            if (deleteEventsAfterDeletion != null && bool.TryParse(deleteEventsAfterDeletion, out var deleteVal))
                deleteFlag = deleteVal;

            var calendar = new Calendar
            {
                Name = name,
                Color = color,
                Short = short_,
                AllowedGroups = allowedGroupIds?.Select(id => new MemberGroup { Id = id }).ToArray(),
                DeleteEventsAfterDeletion = deleteFlag
            };
            var created = await client.CreateCalendarAsync(calendar, ct);
            return JsonSerializer.Serialize(created, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Updates an existing calendar. Only the provided fields are modified (PATCH semantics).
    /// </summary>
    /// <param name="id">The unique identifier of the calendar to update.</param>
    /// <param name="name">Optional new name.</param>
    /// <param name="color">Optional new hex color value.</param>
    /// <param name="short_">Optional new short name.</param>
    /// <param name="allowedGroupIds">Optional new array of member group IDs.</param>
    /// <param name="deleteEventsAfterDeletion">Optional new value for delete-events flag.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A JSON string of the updated calendar, or an error message.</returns>
    [McpServerTool(Name = "update_calendar"), Description("Update a calendar (only provided fields are changed)")]
    public async Task<string> UpdateCalendar(
        [Description("The ID of the calendar")] long id,
        [Description("The new name")] string? name,
        [Description("The new hex color value")] string? color,
        [Description("The new short name")] string? short_,
        [Description("New array of member group IDs")] long[]? allowedGroupIds,
        [Description("New value for delete-events-after-deletion flag (true/false)")] string? deleteEventsAfterDeletion,
        CancellationToken ct)
    {
        try
        {
            var patch = new Dictionary<string, object>();
            if (HasValue(name)) patch[CalendarFields.Name] = name!;
            if (HasValue(color)) patch[CalendarFields.Color] = color!;
            if (HasValue(short_)) patch[CalendarFields.Short] = short_!;
            if (allowedGroupIds != null)
                patch[CalendarFields.AllowedGroups] = allowedGroupIds.Select(gid => new MemberGroup { Id = gid }).ToArray();
            if (HasValue(deleteEventsAfterDeletion) && bool.TryParse(deleteEventsAfterDeletion, out var deleteVal)) patch[CalendarFields.DeleteEventsAfterDeletion] = deleteVal;

            var updated = await client.UpdateCalendarAsync(id, patch, ct);
            return JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.GetType().Name}: {ex.Message}\nInner: {ex.InnerException?.Message}";
        }
    }

    /// <summary>
    /// Deletes a calendar by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the calendar to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A confirmation message, or an error message.</returns>
    [McpServerTool(Name = "delete_calendar"), Description("Delete a calendar. Only authorized users are able to perform this action!")]
    public async Task<string> DeleteCalendar(
        [Description("The ID of the calendar")] long id, CancellationToken ct)
    {
        try
        {
            await client.DeleteCalendarAsync(id, ct);
            return $"Calendar with ID {id} has been deleted.";
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
