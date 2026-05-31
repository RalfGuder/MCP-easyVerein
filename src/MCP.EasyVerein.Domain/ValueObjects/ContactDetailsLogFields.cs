namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Contact-Details-Log API field names used in JSON serialization and query building.</summary>
internal static class ContactDetailsLogFields
{
    /// <summary>API field name for the unique contact-details-log identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the creator (foreign key to the user who created the entry; v2.0 renamed from 'author').</summary>
    internal const string Creator = "creator";

    /// <summary>API field name for the creator's display name (read-only, max 600 characters).</summary>
    internal const string CreatorName = "creatorName";

    /// <summary>API field name for the kind of log (max 20 chars; e.g. 'Balance', 'Custom', 'Membership', 'Email', 'Dsgvo', 'Articles'; default 'Custom').</summary>
    internal const string Kind = "kind";

    /// <summary>API field name for the related address (foreign key to contact-details).</summary>
    internal const string RelatedAddress = "relatedAddress";

    /// <summary>API field name for the related file path (max 360 characters).</summary>
    internal const string RelatedFile = "relatedFile";

    /// <summary>API field name for the date and time of the logged event.</summary>
    internal const string Date = "date";

    /// <summary>API field name for the log description text.</summary>
    internal const string Description = "description";

    /// <summary>API field name for the shared flag (show the log to the related member; default false).</summary>
    internal const string Shared = "shared";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for the date greater-or-equal filter.</summary>
    internal const string DateGte = "date__gte";

    /// <summary>API query parameter for the date less-or-equal filter.</summary>
    internal const string DateLte = "date__lte";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";
}
