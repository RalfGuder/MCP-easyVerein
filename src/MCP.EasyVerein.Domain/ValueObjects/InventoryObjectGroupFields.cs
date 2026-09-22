namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein inventory object group API field names used in JSON serialization and query building.</summary>
internal static class InventoryObjectGroupFields
{
    /// <summary>API field name for the unique group identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the owning organization reference (read-only).</summary>
    internal const string Org = "org";

    /// <summary>API field name for the date the group is removed from the wastebasket (read-only).</summary>
    internal const string DeleteAfterDate = "_deleteAfterDate";

    /// <summary>API field name for the name of the user who deleted the group (read-only).</summary>
    internal const string DeletedBy = "_deletedBy";

    /// <summary>API field name for the creation timestamp (read-only).</summary>
    internal const string CreatedAt = "created_at";

    /// <summary>API field name for the last-update timestamp (read-only).</summary>
    internal const string UpdatedAt = "updated_at";

    /// <summary>API field name for the group name (required, max 200 characters).</summary>
    internal const string Name = "name";

    /// <summary>API field name for the hex color (required for groups, max 7 characters).</summary>
    internal const string Color = "color";

    /// <summary>API field name for the short label (required for groups, max 4 characters).</summary>
    internal const string Short = "short";

    /// <summary>API field name for the items linked to the group (read-only).</summary>
    internal const string LinkedItems = "linkedItems";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for filtering by soft-deleted state.</summary>
    internal const string Deleted = "deleted";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search.</summary>
    internal const string Search = "search";
}
