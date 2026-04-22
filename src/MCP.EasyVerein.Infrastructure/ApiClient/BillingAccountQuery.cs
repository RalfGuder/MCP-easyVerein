using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the billing-account API endpoint with field selection and filters.
/// </summary>
internal class BillingAccountQuery
{
    /// <summary>Gets or sets an optional name filter (exact match).</summary>
    internal string? Name { get; set; }

    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional SKR chart filter (exact match).</summary>
    internal string? Skr { get; set; }

    /// <summary>Gets or sets an optional comma-separated list of SKR charts filter.</summary>
    internal string? SkrIn { get; set; }

    /// <summary>Gets or sets an optional number greater-or-equal filter.</summary>
    internal string? NumberGte { get; set; }

    /// <summary>Gets or sets an optional number less-or-equal filter.</summary>
    internal string? NumberLte { get; set; }

    /// <summary>Gets or sets an optional deleted-flag filter ("true"/"false").</summary>
    internal string? Deleted { get; set; }

    /// <summary>Gets or sets an optional accountingPlan-is-null filter ("true"/"false").</summary>
    internal string? AccountingPlanIsNull { get; set; }

    /// <summary>Gets or sets an optional filter to show only own billing accounts ("true"/"false").</summary>
    internal string? ShowOwnBillingAccounts { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms.</summary>
    internal string[]? Search { get; set; }

    /// <summary>The field selection query requesting all billing-account fields.</summary>
    private const string FieldQuery =
        "query=" +
        "{" +
            BillingAccountFields.Id + "," +
            BillingAccountFields.Name + "," +
            BillingAccountFields.Number + "," +
            BillingAccountFields.DefaultSphere + "," +
            BillingAccountFields.ExcludeInEur + "," +
            BillingAccountFields.Skr + "," +
            BillingAccountFields.Deleted + "," +
            BillingAccountFields.LinkedBookings +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(Name))
            parts.Add($"{BillingAccountFields.Name}={Uri.EscapeDataString(Name)}");
        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{BillingAccountFields.IdIn}={Uri.EscapeDataString(IdIn)}");
        if (!string.IsNullOrEmpty(Skr))
            parts.Add($"{BillingAccountFields.SkrFilter}={Uri.EscapeDataString(Skr)}");
        if (!string.IsNullOrEmpty(SkrIn))
            parts.Add($"{BillingAccountFields.SkrIn}={Uri.EscapeDataString(SkrIn)}");
        if (!string.IsNullOrEmpty(NumberGte))
            parts.Add($"{BillingAccountFields.NumberGte}={Uri.EscapeDataString(NumberGte)}");
        if (!string.IsNullOrEmpty(NumberLte))
            parts.Add($"{BillingAccountFields.NumberLte}={Uri.EscapeDataString(NumberLte)}");
        if (!string.IsNullOrEmpty(Deleted))
            parts.Add($"{BillingAccountFields.DeletedFilter}={Uri.EscapeDataString(Deleted)}");
        if (!string.IsNullOrEmpty(AccountingPlanIsNull))
            parts.Add($"{BillingAccountFields.AccountingPlanIsNull}={Uri.EscapeDataString(AccountingPlanIsNull)}");
        if (!string.IsNullOrEmpty(ShowOwnBillingAccounts))
            parts.Add($"{BillingAccountFields.ShowOwnBillingAccounts}={Uri.EscapeDataString(ShowOwnBillingAccounts)}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{BillingAccountFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{BillingAccountFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
