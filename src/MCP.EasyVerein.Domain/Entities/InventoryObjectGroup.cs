using System.Text.Json;
using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a group used to categorize inventory objects in the easyVerein API.
/// </summary>
public class InventoryObjectGroup : IHasId
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(InventoryObjectGroupFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the owning organization URL reference (read-only). Maps to API field '<c>org</c>'.</summary>
    [JsonPropertyName(InventoryObjectGroupFields.Org)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Org { get; set; }

    /// <summary>Gets or sets the date the group is removed from the wastebasket (read-only). Maps to API field '<c>_deleteAfterDate</c>'.</summary>
    [JsonPropertyName(InventoryObjectGroupFields.DeleteAfterDate)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? DeleteAfterDate { get; set; }

    /// <summary>Gets or sets the name of the user who deleted the group (read-only). Maps to API field '<c>_deletedBy</c>'.</summary>
    [JsonPropertyName(InventoryObjectGroupFields.DeletedBy)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DeletedBy { get; set; }

    /// <summary>Gets or sets the creation timestamp (read-only). Maps to API field '<c>created_at</c>'.</summary>
    [JsonPropertyName(InventoryObjectGroupFields.CreatedAt)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Gets or sets the last-update timestamp (read-only). Maps to API field '<c>updated_at</c>'.</summary>
    [JsonPropertyName(InventoryObjectGroupFields.UpdatedAt)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>Gets or sets the group name (required, max 200 chars). Maps to API field '<c>name</c>'.</summary>
    [JsonPropertyName(InventoryObjectGroupFields.Name)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    /// <summary>Gets or sets the hex color (required, max 7 chars). Maps to API field '<c>color</c>'.</summary>
    [JsonPropertyName(InventoryObjectGroupFields.Color)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Color { get; set; }

    /// <summary>Gets or sets the short label (required, max 4 chars). Maps to API field '<c>short</c>'.</summary>
    [JsonPropertyName(InventoryObjectGroupFields.Short)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Short { get; set; }

    /// <summary>Gets or sets the items linked to the group as raw JSON (read-only). Maps to API field '<c>linkedItems</c>'.</summary>
    [JsonPropertyName(InventoryObjectGroupFields.LinkedItems)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? LinkedItems { get; set; }
}
