using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a DOSB sport discipline from the easyVerein API (read-only resource).
/// </summary>
public class DosbSport : IHasId
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(DosbSportFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the title of the DOSB discipline (max 70 chars). Maps to API field '<c>title</c>'.</summary>
    [JsonPropertyName(DosbSportFields.Title)]
    public string? Title { get; set; }

    /// <summary>Gets or sets the DOSB sport number (max 50 chars). Maps to API field '<c>sportNumber</c>'.</summary>
    [JsonPropertyName(DosbSportFields.SportNumber)]
    public string? SportNumber { get; set; }

    /// <summary>Gets or sets the federation number (max 50 chars). Maps to API field '<c>federationNumber</c>'.</summary>
    [JsonPropertyName(DosbSportFields.FederationNumber)]
    public string? FederationNumber { get; set; }

    /// <summary>Gets or sets the owning organization URL reference. Maps to API field '<c>org</c>'.</summary>
    [JsonPropertyName(DosbSportFields.Org)]
    public string? Org { get; set; }

    /// <summary>Gets or sets the creation timestamp (set by the API). Maps to API field '<c>created_at</c>'.</summary>
    [JsonPropertyName(DosbSportFields.CreatedAt)]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Gets or sets the last-update timestamp (set by the API). Maps to API field '<c>updated_at</c>'.</summary>
    [JsonPropertyName(DosbSportFields.UpdatedAt)]
    public DateTimeOffset? UpdatedAt { get; set; }
}
