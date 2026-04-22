namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Billing Account API field names used in JSON serialization and query building.</summary>
internal static class BillingAccountFields
{
    /// <summary>API field name for the unique billing account identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the billing account name.</summary>
    internal const string Name = "name";

    /// <summary>API field name for the SKR account number.</summary>
    internal const string Number = "number";

    /// <summary>API field name for the default SKR 42 sphere.</summary>
    internal const string DefaultSphere = "defaultSphere";

    /// <summary>API field name for the exclude-in-EUR flag.</summary>
    internal const string ExcludeInEur = "excludeInEur";

    /// <summary>API field name for the SKR chart (e.g. SKR 42).</summary>
    internal const string Skr = "skr";

    /// <summary>API field name for the soft-delete flag.</summary>
    internal const string Deleted = "deleted";

    /// <summary>API field name for the virtual linked-bookings counter (GET only).</summary>
    internal const string LinkedBookings = "linkedBookings";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for filtering by SKR chart (exact).</summary>
    internal const string SkrFilter = "skr";

    /// <summary>API query parameter for filtering by a comma-separated list of SKR charts.</summary>
    internal const string SkrIn = "skr__in";

    /// <summary>API query parameter for filtering numbers greater or equal.</summary>
    internal const string NumberGte = "number__gte";

    /// <summary>API query parameter for filtering numbers less or equal.</summary>
    internal const string NumberLte = "number__lte";

    /// <summary>API query parameter for filtering by deleted flag.</summary>
    internal const string DeletedFilter = "deleted";

    /// <summary>API query parameter for filtering where accountingPlan is null.</summary>
    internal const string AccountingPlanIsNull = "accountingPlan__isnull";

    /// <summary>API query parameter for showing only own billing accounts.</summary>
    internal const string ShowOwnBillingAccounts = "showOwnBillingAccounts";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search.</summary>
    internal const string Search = "search";
}
