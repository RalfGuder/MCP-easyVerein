using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a contact-details group (address group) from the easyVerein API.
/// </summary>
public class ContactDetailsGroup : IHasId
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(ContactDetailsGroupFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the group name (required, max 200 chars). Maps to API field '<c>name</c>'.</summary>
    [JsonPropertyName(ContactDetailsGroupFields.Name)]
    public string? Name { get; set; }

    /// <summary>Gets or sets the hex color value (max 7 chars, e.g. '#ff8800'). Maps to API field '<c>color</c>'.</summary>
    [JsonPropertyName(ContactDetailsGroupFields.Color)]
    public string? Color { get; set; }

    /// <summary>Gets or sets the short label (max 4 chars). Maps to API field '<c>short</c>'.</summary>
    [JsonPropertyName(ContactDetailsGroupFields.Short)]
    public string? Short { get; set; }

    /// <summary>Gets or sets the order sequence used for sorting. Maps to API field '<c>orderSequence</c>'.</summary>
    [JsonPropertyName(ContactDetailsGroupFields.OrderSequence)]
    public int? OrderSequence { get; set; }
}
