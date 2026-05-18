using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds the query string for the booking API endpoint, including field selection and optional filters.
/// </summary>
internal class BookingQuery
{
    /// <summary>Gets or sets an optional comma-separated list of booking IDs (maps to API filter '<c>id__in</c>').</summary>
    internal string? IdIn { get; set; }

    /// <summary>Gets or sets optional search terms to filter bookings.</summary>
    public string[]? Search { get; set; }

    /// <summary>Gets or sets an optional exact date filter.</summary>
    public string? Date { get; set; }

    /// <summary>Gets or sets an optional filter for dates greater than the specified value.</summary>
    public string? DateGt { get; set; }

    /// <summary>Gets or sets an optional filter for dates less than the specified value.</summary>
    public string? DateLt { get; set; }

    /// <summary>Gets or sets an optional ordering criterion for the results.</summary>
    public string? Ordering { get; set; }

    /// <summary>The base field selection query requesting all booking fields, without any filters.
    /// Use for single-resource GETs to avoid leaking shared filter state from prior list calls.</summary>
    internal const string FieldQuery =
        "query=" +
        "{" +
            BookingFields.Id + "," +
            BookingFields.Amount + "," +
            BookingFields.BankAccount +
            "{" +
                BankAccountFields.Id +
            "}," +
            BookingFields.BillingAccount + 
            "{" +
                BillingAccountFields.Id +
            "}," +
            BookingFields.Org +
            "{" +
                OrganizationFields.Id +
            "}," +
            BookingFields.Description + "," +
            BookingFields.Date + "," +
            BookingFields.DeleteAfterDate+ "," +
            BookingFields.DeletedBy + "," +
            BookingFields.Receiver + "," +
            BookingFields.Blocked + "," +
            BookingFields.PaymentDifference + "," +
            BookingFields.CounterpartIban + "," +
            BookingFields.CounterpartBic + "," +
            BookingFields.TwingleDonation + "," +
            BookingFields.BookingProject + "," +
            BookingFields.Sphere + "," +
            BookingFields.RelatedInvoice +
            "{" +
                InvoiceFields.Id + 
            "}" +
        "}";

    /// <summary>Returns the complete query string with field selection and any active filters.</summary>
    /// <returns>A URL query string for the booking endpoint.</returns>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (!string.IsNullOrEmpty(IdIn))
        {
            parts.Add($"{BookingFields.IdIn}={Uri.EscapeDataString(IdIn)}");
        }

        if (!string.IsNullOrEmpty(Date))
        {
            parts.Add($"{BookingFields.Date}={Date}");
        }
        if (!string.IsNullOrEmpty(DateGt))
        {
            parts.Add($"{BookingFields.DateGt}={DateGt}");
        }
        if (!string.IsNullOrEmpty(DateLt))
        {
            parts.Add($"{BookingFields.DateLt}={DateLt}");
        }
        if (!string.IsNullOrEmpty(Ordering))
        {
            parts.Add($"{BookingFields.Ordering}={Ordering}");
        }

        if (Search != null && Search.Length != 0)
        {
            parts.Add($"{BookingFields.Search}={Uri.EscapeDataString(string.Join(",", Search))}");
        }

        return string.Join("&", parts);
    }
}