using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a contact-details log entry (change/audit log of contact details) from the easyVerein API.
/// </summary>
public class ContactDetailsLog : IHasId
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(ContactDetailsLogFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the creator (foreign-key id of the user who created the entry). Maps to API field '<c>creator</c>'.</summary>
    [JsonPropertyName(ContactDetailsLogFields.Creator)]
    [JsonConverter(typeof(FlexibleIdConverter))]
    public long? Creator { get; set; }

    /// <summary>Gets or sets the creator's display name (read-only, max 600 chars). Maps to API field '<c>creatorName</c>'.</summary>
    [JsonPropertyName(ContactDetailsLogFields.CreatorName)]
    public string? CreatorName { get; set; }

    /// <summary>Gets or sets the kind of log (e.g. 'Balance', 'Custom', 'Membership', 'Email', 'Dsgvo', 'Articles'). Maps to API field '<c>kind</c>'.</summary>
    [JsonPropertyName(ContactDetailsLogFields.Kind)]
    public string? Kind { get; set; }

    /// <summary>Gets or sets the related address (foreign-key id of the contact-details). Maps to API field '<c>relatedAddress</c>'.</summary>
    [JsonPropertyName(ContactDetailsLogFields.RelatedAddress)]
    [JsonConverter(typeof(FlexibleIdConverter))]
    public long? RelatedAddress { get; set; }

    /// <summary>Gets or sets the related file path (max 360 chars). Maps to API field '<c>relatedFile</c>'.</summary>
    [JsonPropertyName(ContactDetailsLogFields.RelatedFile)]
    public string? RelatedFile { get; set; }

    /// <summary>Gets or sets the date and time of the logged event. Maps to API field '<c>date</c>'.</summary>
    [JsonPropertyName(ContactDetailsLogFields.Date)]
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? Date { get; set; }

    /// <summary>Gets or sets the log description text. Maps to API field '<c>description</c>'.</summary>
    [JsonPropertyName(ContactDetailsLogFields.Description)]
    public string? Description { get; set; }

    /// <summary>Gets or sets the shared flag (show the log to the related member). Maps to API field '<c>shared</c>'.</summary>
    [JsonPropertyName(ContactDetailsLogFields.Shared)]
    public bool? Shared { get; set; }
}
