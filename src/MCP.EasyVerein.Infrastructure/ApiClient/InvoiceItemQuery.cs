using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds query strings for the invoice-item API endpoint with field selection and filters.
/// </summary>
internal class InvoiceItemQuery
{
    /// <summary>Gets or sets an optional comma-separated list of IDs filter.</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets an optional parent-invoice-ID filter.</summary>
    internal string? RelatedInvoice { get; set; }

    /// <summary>Gets or sets the ordering parameter.</summary>
    internal string? Ordering { get; set; }

    /// <summary>Gets or sets the search terms.</summary>
    internal string[]? Search { get; set; }

    /// <summary>The field selection query requesting all invoice-item response fields.</summary>
    private const string FieldQuery =
        "query=" +
        "{" +
            InvoiceItemFields.Id + "," +
            InvoiceItemFields.Org + "," +
            InvoiceItemFields.RelatedInvoice + "," +
            InvoiceItemFields.BillingAccount + "," +
            InvoiceItemFields.TotalPrice + "," +
            InvoiceItemFields.ArticleObject + "," +
            InvoiceItemFields.Quantity + "," +
            InvoiceItemFields.UnitPrice + "," +
            InvoiceItemFields.Title + "," +
            InvoiceItemFields.Description + "," +
            InvoiceItemFields.TaxRate + "," +
            InvoiceItemFields.Gross + "," +
            InvoiceItemFields.TaxName + "," +
            InvoiceItemFields.Sphere + "," +
            InvoiceItemFields.CostCentre + "," +
            InvoiceItemFields.DeductedExistingBalance +
        "}";

    /// <summary>Builds the complete query string from the field selection and active filters.</summary>
    /// <returns>The combined query string ready to be appended to a request URL.</returns>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(IdIn))
            parts.Add($"{InvoiceItemFields.IdIn}={Uri.EscapeDataString(IdIn)}");
        if (!string.IsNullOrEmpty(RelatedInvoice))
            parts.Add($"{InvoiceItemFields.RelatedInvoiceFilter}={Uri.EscapeDataString(RelatedInvoice)}");
        if (!string.IsNullOrEmpty(Ordering))
            parts.Add($"{InvoiceItemFields.Ordering}={Ordering}");
        if (Search != null && Search.Length != 0)
            parts.Add($"{InvoiceItemFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");

        return string.Join("&", parts);
    }
}
