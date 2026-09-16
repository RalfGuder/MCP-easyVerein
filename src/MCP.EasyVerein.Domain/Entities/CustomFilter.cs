using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a saved custom filter (stored filter criteria for a list view) from the easyVerein API.
/// </summary>
public class CustomFilter : IHasId
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(CustomFilterFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the name of the filter (max 64 chars). Maps to API field '<c>name</c>'.</summary>
    [JsonPropertyName(CustomFilterFields.Name)]
    public string? Name { get; set; }

    /// <summary>Gets or sets the filter model defining the filtered table (e.g. 'userFilter', 'bookingFilter'). Maps to API field '<c>model</c>'.</summary>
    [JsonPropertyName(CustomFilterFields.Model)]
    public string? Model { get; set; }

    /// <summary>Gets or sets the filter rules as a raw JSON object (keys 'condition' and 'rules'). Maps to API field '<c>rules</c>'.</summary>
    [JsonPropertyName(CustomFilterFields.Rules)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? Rules { get; set; }

    /// <summary>Gets or sets the creation timestamp (read-only, set by the API). Maps to API field '<c>created_at</c>'.</summary>
    [JsonPropertyName(CustomFilterFields.CreatedAt)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Gets or sets the last-update timestamp (read-only, set by the API). Maps to API field '<c>updated_at</c>'.</summary>
    [JsonPropertyName(CustomFilterFields.UpdatedAt)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? UpdatedAt { get; set; }
}
