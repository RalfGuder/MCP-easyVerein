using MCP.EasyVerein.Domain.Converters;
using MCP.EasyVerein.Domain.Interfaces;
using MCP.EasyVerein.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace MCP.EasyVerein.Domain.Entities;

/// <summary>
/// Represents a booking from the easyVerein API.
/// </summary>
public class Booking : IHasId
{
    /// <summary>
    /// Gets or sets the booking amount. Maps to API field ' <c>amount</c>'.
    /// </summary>
    [JsonPropertyName(BookingFields.Amount)] public decimal? Amount { get; set; }

    /// <summary>
    /// Gets or sets the bank account reference. Maps to API field ' <c>bankAccount</c>'. May arrive as
    /// embedded object or URL reference — handled by <see cref="FlexibleReferenceConverter{T}"/>.
    /// </summary>
    [JsonPropertyName(BookingFields.BankAccount)]
    [JsonConverter(typeof(FlexibleReferenceConverter<BankAccount>))]
    public BankAccount? BankAccount { get; set; }

    /// <summary>
    /// Gets or sets the billing account reference. Maps to API field ' <c>billingAccount</c>'. May arrive as
    /// embedded object or URL reference — handled by <see cref="FlexibleReferenceConverter{T}"/>.
    /// </summary>
    [JsonPropertyName(BookingFields.BillingAccount)]
    [JsonConverter(typeof(FlexibleReferenceConverter<BillingAccount>))]
    public BillingAccount? BillingAccount { get; set; }

    /// <summary>
    /// Gets or sets whether the booking is blocked. Maps to API field ' <c>blocked</c>'.
    /// </summary>
    [JsonPropertyName(BookingFields.Blocked)] public bool? Blocked { get; set; }

    /// <summary>
    /// Gets or sets the booking project reference. Maps to API field ' <c>bookingProject</c>'.
    /// </summary>
    [JsonPropertyName(BookingFields.BookingProject)] public string? BookingProject { get; set; }

    /// <summary>
    /// Gets or sets the counterpart BIC. Maps to API field ' <c>counterpartBic</c>'.
    /// </summary>
    [JsonPropertyName(BookingFields.CounterpartBic)] public string? CounterpartBic { get; set; }

    /// <summary>
    /// Gets or sets the counterpart IBAN. Maps to API field ' <c>counterpartIban</c>'.
    /// </summary>
    [JsonPropertyName(BookingFields.CounterpartIban)] public string? CounterpartIban { get; set; }

    /// <summary>
    /// Gets or sets the booking date. Maps to API field ' <c>date</c>'.
    /// </summary>
    [JsonPropertyName(BookingFields.Date)] public DateTime? Date { get; set; }

    /// <summary>
    /// Gets or sets the date after which the booking is deleted. Maps to API field '<c>_deleteAfterDate</c>'.
    /// </summary>
    [JsonPropertyName(BookingFields.DeleteAfterDate)] public DateTime? DeleteAfterDate { get; set; }

    /// <summary>
    /// Gets or sets the user who deleted the booking. Maps to API field '<c>_deletedBy</c>'.
    /// </summary>
    [JsonPropertyName(BookingFields.DeletedBy)] public string? DeletedBy { get; set; }

    /// <summary>
    /// Gets or sets the description. Maps to API field ' <c>description</c>'.
    /// </summary>
    [JsonPropertyName(BookingFields.Description)] public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier. Maps to API field ' <c>id</c>'.
    /// </summary>
    [JsonPropertyName(BookingFields.Id)] public long Id { get; set; }

    /// <summary>
    /// Gets or sets the organization reference. Maps to API field '<c>org</c>'. May arrive as embedded
    /// object or URL reference — handled by <see cref="FlexibleReferenceConverter{T}"/>.
    /// </summary>
    [JsonPropertyName(BookingFields.Org)]
    [JsonConverter(typeof(FlexibleReferenceConverter<Organization>))]
    public Organization? Org { get; set; }

    /// <summary>
    /// Gets or sets the payment difference. Maps to API field ' <c>paymentDifference</c>'.
    /// </summary>
    [JsonPropertyName(BookingFields.PaymentDifference)] public decimal? PaymentDifference { get; set; }

    /// <summary>
    /// Gets or sets the receiver. Maps to API field ' <c>receiver</c>'.
    /// </summary>
    [JsonPropertyName(BookingFields.Receiver)] public string? Receiver { get; set; }

    /// <summary>
    /// Gets or sets the related invoices. Maps to API field ' <c>relatedInvoice</c>'. Elements may
    /// arrive as embedded invoice objects (v1.7, GET) or URL references (v2.0 PATCH) — handled by
    /// <see cref="FlexibleReferenceListConverter{T}"/>.
    /// </summary>
    [JsonPropertyName(BookingFields.RelatedInvoice)]
    [JsonConverter(typeof(FlexibleReferenceListConverter<Invoice>))]
    public List<Invoice>? RelatedInvoice { get; set; }

    /// <summary>
    /// Gets or sets the sphere (area). Maps to API field ' <c>sphere</c>'.
    /// </summary>
    [JsonPropertyName(BookingFields.Sphere)] public long? Sphere { get; set; }

    /// <summary>
    /// Gets or sets whether this is a Twingle donation. Maps to API field ' <c>twingleDonation</c>'.
    /// </summary>
    [JsonPropertyName(BookingFields.TwingleDonation)] public bool? TwingleDonation { get; set; }
}