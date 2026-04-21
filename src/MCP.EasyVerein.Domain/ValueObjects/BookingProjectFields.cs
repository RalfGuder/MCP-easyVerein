namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Booking Project API field names used in JSON serialization and query building.</summary>
internal static class BookingProjectFields
{
    /// <summary>API field name for the unique booking project identifier.</summary>
    internal const string Id = "id";

    /// <summary>API field name for the booking project name.</summary>
    internal const string Name = "name";

    /// <summary>API field name for the hex color value.</summary>
    internal const string Color = "color";

    /// <summary>API field name for the short label (max 4 characters).</summary>
    internal const string Short = "short";

    /// <summary>API field name for the project budget.</summary>
    internal const string Budget = "budget";

    /// <summary>API field name for the completion flag.</summary>
    internal const string Completed = "completed";

    /// <summary>API field name for the project cost centre.</summary>
    internal const string ProjectCostCentre = "projectCostCentre";

    /// <summary>API query parameter for filtering by a comma-separated list of IDs.</summary>
    internal const string IdIn = "id__in";

    /// <summary>API query parameter for filtering budgets greater than a value.</summary>
    internal const string BudgetGt = "budget__gt";

    /// <summary>API query parameter for filtering budgets less than a value.</summary>
    internal const string BudgetLt = "budget__lt";

    /// <summary>API query parameter for ordering results.</summary>
    internal const string Ordering = "ordering";

    /// <summary>API query parameter for full-text search.</summary>
    internal const string Search = "search";
}
