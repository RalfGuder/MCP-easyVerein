using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the booking-project API endpoint with field selection and filters.
/// </summary>
internal class BookingProjectQuery
{
    /// <summary>Gets or sets an optional name filter (exact match).</summary>
    internal string? Name { get; set; }

    /// <summary>Gets or sets an optional short label filter (exact match).</summary>
    internal string? Short { get; set; }

    /// <summary>Gets or sets an optional completed-flag filter ("true"/"false").</summary>
    internal string? Completed { get; set; }

    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional budget greater-than filter.</summary>
    internal string? BudgetGt { get; set; }

    /// <summary>Gets or sets an optional budget less-than filter.</summary>
    internal string? BudgetLt { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms.</summary>
    internal string[]? Search { get; set; }

    /// <summary>The field selection query requesting all booking-project fields.</summary>
    private const string FieldQuery =
        "query=" +
        "{" +
            BookingProjectFields.Id + "," +
            BookingProjectFields.Name + "," +
            BookingProjectFields.Color + "," +
            BookingProjectFields.Short + "," +
            BookingProjectFields.Budget + "," +
            BookingProjectFields.Completed + "," +
            BookingProjectFields.ProjectCostCentre +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(Name))
            parts.Add($"{BookingProjectFields.Name}={Uri.EscapeDataString(Name)}");
        if (!string.IsNullOrEmpty(Short))
            parts.Add($"{BookingProjectFields.Short}={Uri.EscapeDataString(Short)}");
        if (!string.IsNullOrEmpty(Completed))
            parts.Add($"{BookingProjectFields.Completed}={Uri.EscapeDataString(Completed)}");
        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{BookingProjectFields.IdIn}={Uri.EscapeDataString(IdIn)}");
        if (!string.IsNullOrEmpty(BudgetGt))
            parts.Add($"{BookingProjectFields.BudgetGt}={Uri.EscapeDataString(BudgetGt)}");
        if (!string.IsNullOrEmpty(BudgetLt))
            parts.Add($"{BookingProjectFields.BudgetLt}={Uri.EscapeDataString(BudgetLt)}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{BookingProjectFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{BookingProjectFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
