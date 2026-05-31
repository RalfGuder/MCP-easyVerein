using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.Converters;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a custom field (user-defined field) from the easyVerein API.
/// </summary>
public class CustomField : IHasId
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(CustomFieldFields.Id)]
    public long Id { get; set; }

    /// <summary>Gets or sets the name of the group or custom field (required, max 200 chars). Maps to API field '<c>name</c>'.</summary>
    [JsonPropertyName(CustomFieldFields.Name)]
    public string? Name { get; set; }

    /// <summary>Gets or sets the hex color value (max 7 chars; required for groups). Maps to API field '<c>color</c>'.</summary>
    [JsonPropertyName(CustomFieldFields.Color)]
    public string? Color { get; set; }

    /// <summary>Gets or sets the short label (max 4 chars; required for groups, must be unique). Maps to API field '<c>short</c>'.</summary>
    [JsonPropertyName(CustomFieldFields.Short)]
    public string? Short { get; set; }

    /// <summary>Gets or sets the field type code (e.g. 'T','F','Z','D','C','R','S','A','B','M'; default 'T'). Maps to API field '<c>settings_type</c>'.</summary>
    [JsonPropertyName(CustomFieldFields.SettingsType)]
    public string? SettingsType { get; set; }

    /// <summary>Gets or sets the kind code (for custom fields: 'E'=members, 'H'=events, 'J'=contact-details, 'I'=inventory). Maps to API field '<c>kind</c>'.</summary>
    [JsonPropertyName(CustomFieldFields.Kind)]
    public string? Kind { get; set; }

    /// <summary>Gets or sets the description (max 124 chars). Maps to API field '<c>description</c>'.</summary>
    [JsonPropertyName(CustomFieldFields.Description)]
    public string? Description { get; set; }

    /// <summary>Gets or sets additional metadata. Maps to API field '<c>additional</c>'.</summary>
    [JsonPropertyName(CustomFieldFields.Additional)]
    public string? Additional { get; set; }

    /// <summary>Gets or sets the show-in-member-area flag (default false). Maps to API field '<c>member_show</c>'.</summary>
    [JsonPropertyName(CustomFieldFields.MemberShow)]
    public bool? MemberShow { get; set; }

    /// <summary>Gets or sets the editable-in-member-area flag (default false). Maps to API field '<c>member_edit</c>'.</summary>
    [JsonPropertyName(CustomFieldFields.MemberEdit)]
    public bool? MemberEdit { get; set; }

    /// <summary>Gets or sets the GDPR-consent flag (default false). Maps to API field '<c>member_dsgvo</c>'.</summary>
    [JsonPropertyName(CustomFieldFields.MemberDsgvo)]
    public bool? MemberDsgvo { get; set; }

    /// <summary>Gets or sets the position within the tab/group (default 0). Maps to API field '<c>position</c>'.</summary>
    [JsonPropertyName(CustomFieldFields.Position)]
    public int? Position { get; set; }

    /// <summary>Gets or sets the related collection (foreign-key id of the tab/group). Maps to API field '<c>collection</c>'.</summary>
    [JsonPropertyName(CustomFieldFields.Collection)]
    [JsonConverter(typeof(FlexibleIdConverter))]
    public long? Collection { get; set; }

    /// <summary>Gets or sets the needs-admin-approval flag (default false). Maps to API field '<c>needsAdminApproval</c>'.</summary>
    [JsonPropertyName(CustomFieldFields.NeedsAdminApproval)]
    public bool? NeedsAdminApproval { get; set; }
}
