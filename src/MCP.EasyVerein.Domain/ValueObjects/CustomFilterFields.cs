namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Custom-Filter API field names used in JSON serialization and query building.</summary>
internal static class CustomFilterFields
{
    /// <summary>API field name for the unique custom-filter identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the name of the custom filter (max 64 characters).</summary>
    internal const string Name = "name";

    /// <summary>API field name for the filter model defining the filtered table (required on create; e.g. 'userFilter', 'bookingFilter', 'invoiceFilter').</summary>
    internal const string Model = "model";

    /// <summary>API field name for the filter rules (a JSON object with the keys 'condition' and 'rules').</summary>
    internal const string Rules = "rules";

    /// <summary>API field name for the creation timestamp (set automatically).</summary>
    internal const string CreatedAt = "created_at";

    /// <summary>API field name for the last-update timestamp (set automatically).</summary>
    internal const string UpdatedAt = "updated_at";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for filtering by a comma-separated list of models.</summary>
    internal const string ModelIn = "model__in";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search (allowed field: name).</summary>
    internal const string Search = "search";
}
