namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Custom-Field API field names used in JSON serialization and query building.</summary>
internal static class CustomFieldFields
{
    /// <summary>API field name for the unique custom-field identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the name of the group or custom field (required, max 200 characters).</summary>
    internal const string Name = "name";

    /// <summary>API field name for the hex color value (max 7 characters; required for groups only).</summary>
    internal const string Color = "color";

    /// <summary>API field name for the short label (max 4 characters; required for groups only, must be unique).</summary>
    internal const string Short = "short";

    /// <summary>API field name for the field type code (max 1 char; e.g. 'T','F','Z','D','C','R','S','A','B','M'; default 'T').</summary>
    internal const string SettingsType = "settings_type";

    /// <summary>API field name for the kind code (max 2 chars; for custom fields: 'E'=members, 'H'=events, 'J'=contact-details, 'I'=inventory).</summary>
    internal const string Kind = "kind";

    /// <summary>API field name for the description (max 124 characters).</summary>
    internal const string Description = "description";

    /// <summary>API field name for additional metadata.</summary>
    internal const string Additional = "additional";

    /// <summary>API field name for the show-in-member-area flag (default false).</summary>
    internal const string MemberShow = "member_show";

    /// <summary>API field name for the editable-in-member-area flag (default false).</summary>
    internal const string MemberEdit = "member_edit";

    /// <summary>API field name for the GDPR-consent flag (cannot be changed after member approval; default false).</summary>
    internal const string MemberDsgvo = "member_dsgvo";

    /// <summary>API field name for the position within the tab/group (default 0).</summary>
    internal const string Position = "position";

    /// <summary>API field name for the related collection (foreign key to the tab/group).</summary>
    internal const string Collection = "collection";

    /// <summary>API field name for the needs-admin-approval flag (default false).</summary>
    internal const string NeedsAdminApproval = "needsAdminApproval";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for filtering by a comma-separated list of field types.</summary>
    internal const string SettingsTypeIn = "settings_type__in";

    /// <summary>API query parameter for the soft-delete filter.</summary>
    internal const string Deleted = "deleted";

    /// <summary>API query parameter for the no-collection filter.</summary>
    internal const string CollectionIsnull = "collection__isnull";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search (allowed fields: name, color, short).</summary>
    internal const string Search = "search";
}
