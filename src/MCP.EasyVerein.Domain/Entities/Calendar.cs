using MCP.EasyVerein.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a calendar from the easyVerein API.
/// </summary>
public class Calendar
{
    /// <summary>
    /// Gets or sets the unique identifier. Maps to API field '<c>id</c>'.
    /// </summary>
    [JsonPropertyName(CalendarFields.Id)] public long Id { get; set; }

    /// <summary>
    /// Gets or sets the calendar name. Maps to API field '<c>name</c>'.
    /// </summary>
    [JsonPropertyName(CalendarFields.Name)] public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the calendar color as hex value. Maps to API field '<c>color</c>'.
    /// </summary>
    [JsonPropertyName(CalendarFields.Color)] public string? Color { get; set; }

    /// <summary>
    /// Gets or sets the calendar short name (abbreviation). Maps to API field '<c>short</c>'.
    /// </summary>
    [JsonPropertyName(CalendarFields.Short)] public string? Short { get; set; }

    /// <summary>
    /// Gets or sets the allowed member groups. Maps to API field '<c>allowedGroups</c>'.
    /// </summary>
    [JsonPropertyName(CalendarFields.AllowedGroups)] public MemberGroup[]? AllowedGroups { get; set; }

    /// <summary>
    /// Gets or sets the count of linked items. Maps to API field '<c>linkedItems</c>'.
    /// </summary>
    [JsonPropertyName(CalendarFields.LinkedItems)] public int? LinkedItems { get; set; }

    /// <summary>
    /// Gets or sets whether events are deleted after calendar deletion. Maps to API field '<c>deleteEventsAfterDeletion</c>'.
    /// </summary>
    [JsonPropertyName(CalendarFields.DeleteEventsAfterDeletion)] public bool? DeleteEventsAfterDeletion { get; set; }
}
