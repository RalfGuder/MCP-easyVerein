using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Infrastructure.ApiClient;

/// <summary>
/// Builds the query string for the booking API endpoint, including field selection and optional filters.
/// </summary>
internal class BookingQuery
{
    /// <summary>Gets or sets an optional booking identifier filter.</summary>
    internal long? Id { get; set; }

    /// <summary>The base field selection query requesting all booking fields.</summary>
    private const string FieldQuery =
        "query=" +
        "{" +
            BookingFields.Id + "," +
            BookingFields.Amount + "," +
            BookingFields.BankAccount + "," +
            BookingFields.BillingAccount + "," +
            BookingFields.Description + "," +
            BookingFields.Date + "," +
            BookingFields.Receiver + "," +
            BookingFields.BillingId + "," +
            BookingFields.Blocked + "," +
            BookingFields.PaymentDifference + "," +
            BookingFields.CounterpartIban + "," +
            BookingFields.CounterpartBic + "," +
            BookingFields.TwingleDonation + "," +
            BookingFields.BookingProject + "," +
            BookingFields.Sphere + "," +
            BookingFields.RelatedInvoice +
        "}";

    /// <summary>Returns the complete query string with field selection and any active filters.</summary>
    /// <returns>A URL query string for the booking endpoint.</returns>
    public override string ToString()
    {
        var parts = new List<string> { FieldQuery };

        if (Id != null)
            parts.Add($"{BookingFields.Id}={Id}");

        return string.Join("&", parts);
    }
}
