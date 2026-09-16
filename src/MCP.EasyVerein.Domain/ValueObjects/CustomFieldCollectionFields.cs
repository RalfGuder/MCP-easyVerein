namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Custom-Field-Collection API field names used in JSON serialization and query building.</summary>
internal static class CustomFieldCollectionFields
{
    /// <summary>API field name for the unique custom-field-collection identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the name of the collection (required, max 200 characters).</summary>
    internal const string Name = "name";

    /// <summary>API field name for the sort order of the collection.</summary>
    internal const string OrderSequence = "orderSequence";

    /// <summary>API field name for the position within the member profile tab.</summary>
    internal const string Position = "position";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search.</summary>
    internal const string Search = "search";
}
