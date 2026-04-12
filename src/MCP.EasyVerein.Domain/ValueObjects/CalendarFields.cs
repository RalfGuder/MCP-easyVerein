namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>
/// Constants for easyVerein Calendar API field names used in JSON serialization and query building.
/// </summary>
internal static class CalendarFields
{
    /// <summary>API field name for the unique calendar identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the calendar name.</summary>
    internal const string Name = "name";

    /// <summary>API field name for the calendar color (hex value).</summary>
    internal const string Color = "color";

    /// <summary>API field name for the calendar short name (abbreviation).</summary>
    internal const string Short = "short";

    /// <summary>API field name for the allowed member groups.</summary>
    internal const string AllowedGroups = "allowedGroups";

    /// <summary>API field name for the count of linked items.</summary>
    internal const string LinkedItems = "linkedItems";

    /// <summary>API field name for whether events are deleted after calendar deletion.</summary>
    internal const string DeleteEventsAfterDeletion = "deleteEventsAfterDeletion";

    /// <summary>API filter field name for name negation.</summary>
    internal const string NameNot = "name__not";

    /// <summary>API filter field name for color negation.</summary>
    internal const string ColorNot = "color__not";

    /// <summary>API filter field name for short name negation.</summary>
    internal const string ShortNot = "short__not";

    /// <summary>API filter field name for filtering by multiple IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for result ordering.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search.</summary>
    internal const string Search = "search";
}
