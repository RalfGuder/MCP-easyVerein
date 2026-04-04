using System.Text.Json.Serialization;
using MCP.EasyVerein.Domain.ValueObjects;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>Represents a booking from the easyVerein API.</summary>
public class Booking
{
    /// <summary>Gets or sets the unique identifier. Maps to API field '<c>id</c>'.</summary>
    [JsonPropertyName(BookingFields.Id)] public long Id { get; set; }

    /// <summary>Gets or sets the booking amount. Maps to API field '<c>amount</c>'.</summary>
    [JsonPropertyName(BookingFields.Amount)] public decimal? Amount { get; set; }

    /// <summary>Gets or sets the bank account reference. Maps to API field '<c>bankAccount</c>'.</summary>
    [JsonPropertyName(BookingFields.BankAccount)] public long? BankAccount { get; set; }

    /// <summary>Gets or sets the billing account reference. Maps to API field '<c>billingAccount</c>'.</summary>
    [JsonPropertyName(BookingFields.BillingAccount)] public long? BillingAccount { get; set; }

    /// <summary>Gets or sets the description. Maps to API field '<c>description</c>'.</summary>
    [JsonPropertyName(BookingFields.Description)] public string? Description { get; set; }

    /// <summary>Gets or sets the booking date. Maps to API field '<c>date</c>'.</summary>
    [JsonPropertyName(BookingFields.Date)] public DateTime? Date { get; set; }

    /// <summary>Gets or sets the receiver. Maps to API field '<c>receiver</c>'.</summary>
    [JsonPropertyName(BookingFields.Receiver)] public string? Receiver { get; set; }

    /// <summary>Gets or sets the billing ID. Maps to API field '<c>billingId</c>'.</summary>
    [JsonPropertyName(BookingFields.BillingId)] public string? BillingId { get; set; }

    /// <summary>Gets or sets whether the booking is blocked. Maps to API field '<c>blocked</c>'.</summary>
    [JsonPropertyName(BookingFields.Blocked)] public bool Blocked { get; set; }

    /// <summary>Gets or sets the payment difference. Maps to API field '<c>paymentDifference</c>'.</summary>
    [JsonPropertyName(BookingFields.PaymentDifference)] public decimal? PaymentDifference { get; set; }

    /// <summary>Gets or sets the counterpart IBAN. Maps to API field '<c>counterpartIban</c>'.</summary>
    [JsonPropertyName(BookingFields.CounterpartIban)] public string? CounterpartIban { get; set; }

    /// <summary>Gets or sets the counterpart BIC. Maps to API field '<c>counterpartBic</c>'.</summary>
    [JsonPropertyName(BookingFields.CounterpartBic)] public string? CounterpartBic { get; set; }

    /// <summary>Gets or sets whether this is a Twingle donation. Maps to API field '<c>twingleDonation</c>'.</summary>
    [JsonPropertyName(BookingFields.TwingleDonation)] public bool TwingleDonation { get; set; }

    /// <summary>Gets or sets the booking project reference. Maps to API field '<c>bookingProject</c>'.</summary>
    [JsonPropertyName(BookingFields.BookingProject)] public string? BookingProject { get; set; }

    /// <summary>Gets or sets the sphere (area). Maps to API field '<c>sphere</c>'.</summary>
    [JsonPropertyName(BookingFields.Sphere)] public string? Sphere { get; set; }

    /// <summary>Gets or sets the related invoice IDs. Maps to API field '<c>relatedInvoice</c>'.</summary>
    [JsonPropertyName(BookingFields.RelatedInvoice)] public List<long>? RelatedInvoice { get; set; }
}
