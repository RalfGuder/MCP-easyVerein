using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;

namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>
/// Represents the author of a feature request as embedded by the easyVerein API.
/// </summary>
public class FeatureRequestAuthor
{
    /// <summary>
    /// Gets or sets the user identifier of the author. Maps to API field '<c>id</c>'.
    /// <c>null</c> for anonymized authors, where the API sends the string <c>"Unbekannt"</c>.
    /// </summary>
    [JsonPropertyName(FeatureRequestFields.Id)]
    [JsonConverter(typeof(FlexibleIdConverter))]
    public long? Id { get; set; }

    /// <summary>Gets or sets the organization of the author. Maps to API field '<c>org</c>'.</summary>
    [JsonPropertyName(FeatureRequestFields.Org)]
    public FeatureRequestAuthorOrganization? Org { get; set; }
}

/// <summary>
/// Represents the organization of a feature-request author as embedded by the easyVerein API.
/// </summary>
public class FeatureRequestAuthorOrganization
{
    /// <summary>
    /// Gets or sets the organization identifier. Maps to API field '<c>id</c>'.
    /// <c>null</c> for anonymized authors, where the API sends an empty string.
    /// </summary>
    [JsonPropertyName(FeatureRequestFields.Id)]
    [JsonConverter(typeof(FlexibleIdConverter))]
    public long? Id { get; set; }

    /// <summary>Gets or sets the short name of the organization. Maps to API field '<c>short</c>'.</summary>
    [JsonPropertyName(FeatureRequestFields.Short)]
    public string? Short { get; set; }

    /// <summary>Gets or sets the name of the organization. Maps to API field '<c>name</c>'.</summary>
    [JsonPropertyName(FeatureRequestFields.Name)]
    public string? Name { get; set; }
}
