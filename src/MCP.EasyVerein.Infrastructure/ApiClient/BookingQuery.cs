using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds the query string for the booking API endpoint, including field selection and optional filters.
/// </summary>
internal class BookingQuery
{
    /// <summary>Gets or sets an optional booking identifier filter.</summary>
    internal long? Id { get; set; }

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

    /// <summary>The base field selection query requesting all booking fields.</summary>
    private const string FieldQuery =
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

        if (Id != null)
        {
            parts.Add($"{BookingFields.Id}={Id}");
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