using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the custom-tax-rate API endpoint with field selection and filters.
/// </summary>
internal class CustomTaxRateQuery
{
    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional tax-rate label filter (exact match).</summary>
    internal string? TaxName { get; set; }

    /// <summary>Gets or sets an optional tax-rate label to exclude.</summary>
    internal string? TaxNameNe { get; set; }

    /// <summary>Gets or sets an optional percentage filter.</summary>
    internal string? CustomTaxRate { get; set; }

    /// <summary>Gets or sets an optional percentage to exclude.</summary>
    internal string? CustomTaxRateNe { get; set; }

    /// <summary>Gets or sets an optional standard-rate filter (true = standard, false = organization-specific).</summary>
    internal bool? OrgIsnull { get; set; }

    /// <summary>Gets or sets an optional soft-delete filter.</summary>
    internal bool? Deleted { get; set; }

    /// <summary>Gets or sets an optional filter returning only usable rates.</summary>
    internal bool? ShowAllowedToUse { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms (allowed field: taxName).</summary>
    internal string[]? Search { get; set; }

    /// <summary>Field selection only, without any filters. Use for single-resource GETs.</summary>
    internal const string FieldQuery =
        "query=" +
        "{" +
            CustomTaxRateFields.Id + "," +
            CustomTaxRateFields.TaxName + "," +
            CustomTaxRateFields.CustomTaxRate + "," +
            CustomTaxRateFields.CountryCode + "," +
            CustomTaxRateFields.Org + "," +
            CustomTaxRateFields.CreatedAt + "," +
            CustomTaxRateFields.UpdatedAt +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{CustomTaxRateFields.IdIn}={Uri.EscapeDataString(IdIn)}");
        if (!string.IsNullOrEmpty(TaxName))
            parts.Add($"{CustomTaxRateFields.TaxName}={Uri.EscapeDataString(TaxName)}");
        if (!string.IsNullOrEmpty(TaxNameNe))
            parts.Add($"{CustomTaxRateFields.TaxNameNe}={Uri.EscapeDataString(TaxNameNe)}");
        if (!string.IsNullOrEmpty(CustomTaxRate))
            parts.Add($"{CustomTaxRateFields.CustomTaxRate}={Uri.EscapeDataString(CustomTaxRate)}");
        if (!string.IsNullOrEmpty(CustomTaxRateNe))
            parts.Add($"{CustomTaxRateFields.CustomTaxRateNe}={Uri.EscapeDataString(CustomTaxRateNe)}");
        if (OrgIsnull.HasValue)
            parts.Add($"{CustomTaxRateFields.OrgIsnull}={(OrgIsnull.Value ? "true" : "false")}");
        if (Deleted.HasValue)
            parts.Add($"{CustomTaxRateFields.Deleted}={(Deleted.Value ? "true" : "false")}");
        if (ShowAllowedToUse.HasValue)
            parts.Add($"{CustomTaxRateFields.ShowAllowedToUse}={(ShowAllowedToUse.Value ? "true" : "false")}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{CustomTaxRateFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{CustomTaxRateFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
