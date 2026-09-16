namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Custom-Tax-Rate API field names used in JSON serialization and query building.</summary>
internal static class CustomTaxRateFields
{
    /// <summary>API field name for the unique tax-rate identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the tax-rate label (required, max 600 characters).</summary>
    internal const string TaxName = "taxName";

    /// <summary>API field name for the tax-rate percentage (required, 0–100; returned as string-encoded decimal).</summary>
    internal const string CustomTaxRate = "customTaxRate";

    /// <summary>API field name for the country code of a standard tax rate (read-only, max 2 characters).</summary>
    internal const string CountryCode = "countryCode";

    /// <summary>API field name for the owning organization reference (read-only; null for standard rates).</summary>
    internal const string Org = "org";

    /// <summary>API field name for the creation timestamp (set automatically).</summary>
    internal const string CreatedAt = "created_at";

    /// <summary>API field name for the last-update timestamp (set automatically).</summary>
    internal const string UpdatedAt = "updated_at";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for excluding a tax-rate label.</summary>
    internal const string TaxNameNe = "taxName__ne";

    /// <summary>API query parameter for excluding a tax-rate percentage.</summary>
    internal const string CustomTaxRateNe = "customTaxRate__ne";

    /// <summary>API query parameter for filtering standard rates (true) or organization-specific rates (false).</summary>
    internal const string OrgIsnull = "org__isnull";

    /// <summary>API query parameter for the soft-delete filter.</summary>
    internal const string Deleted = "deleted";

    /// <summary>API query parameter for returning only the rates the organization is allowed to use.</summary>
    internal const string ShowAllowedToUse = "showAllowedToUse";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search (allowed field: taxName).</summary>
    internal const string Search = "search";
}
