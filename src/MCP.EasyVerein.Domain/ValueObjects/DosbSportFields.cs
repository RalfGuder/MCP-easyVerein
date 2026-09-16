namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein DOSB-Sport API field names used in JSON deserialization and query building.</summary>
internal static class DosbSportFields
{
    /// <summary>API field name for the unique DOSB-sport identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the title of the DOSB discipline (max 70 characters).</summary>
    internal const string Title = "title";

    /// <summary>API field name for the DOSB sport number (max 50 characters).</summary>
    internal const string SportNumber = "sportNumber";

    /// <summary>API field name for the federation number (max 50 characters).</summary>
    internal const string FederationNumber = "federationNumber";

    /// <summary>API field name for the owning organization reference.</summary>
    internal const string Org = "org";

    /// <summary>API field name for the creation timestamp (set automatically).</summary>
    internal const string CreatedAt = "created_at";

    /// <summary>API field name for the last-update timestamp (set automatically).</summary>
    internal const string UpdatedAt = "updated_at";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search.</summary>
    internal const string Search = "search";
}
