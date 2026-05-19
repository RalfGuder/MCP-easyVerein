namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Contact-Details-Group API field names used in JSON serialization and query building.</summary>
internal static class ContactDetailsGroupFields
{
    /// <summary>API field name for the unique contact-details-group identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the group name (required, max 200 characters).</summary>
    internal const string Name = "name";

    /// <summary>API field name for the hex color value (max 7 characters, e.g. '#ff8800').</summary>
    internal const string Color = "color";

    /// <summary>API field name for the short label (max 4 characters).</summary>
    internal const string Short = "short";

    /// <summary>API field name for the order sequence (sort order).</summary>
    internal const string OrderSequence = "orderSequence";

    /// <summary>API query parameter for the soft-delete filter.</summary>
    internal const string Deleted = "deleted";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search (allowed fields: name, short).</summary>
    internal const string Search = "search";
}
