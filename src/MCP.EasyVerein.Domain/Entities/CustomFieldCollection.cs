using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a custom field collection (a group/tab of user-defined fields) from the easyVerein API.
/// </summary>
public class CustomFieldCollection : IHasId
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(CustomFieldCollectionFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the name of the collection (required, max 200 chars). Maps to API field '<c>name</c>'.</summary>
    [JsonPropertyName(CustomFieldCollectionFields.Name)]
    public string? Name { get; set; }

    /// <summary>Gets or sets the sort order of the collection. Maps to API field '<c>orderSequence</c>'.</summary>
    [JsonPropertyName(CustomFieldCollectionFields.OrderSequence)]
    public int? OrderSequence { get; set; }

    /// <summary>Gets or sets the position within the member profile tab. Maps to API field '<c>position</c>'.</summary>
    [JsonPropertyName(CustomFieldCollectionFields.Position)]
    public int? Position { get; set; }
}
