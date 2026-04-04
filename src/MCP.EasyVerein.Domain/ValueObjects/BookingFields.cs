namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>
/// Constants for easyVerein Booking API field names used in JSON serialization.
/// </summary>
internal static class BookingFields
{
    /// <summary>
    /// API field name for the booking amount.
    /// </summary>
    internal const string Amount = "amount";

    /// <summary>
    /// API field name for the bank account reference.
    /// </summary>
    internal const string BankAccount = "bankAccount";

    /// <summary>
    /// API field name for the billing account reference.
    /// </summary>
    internal const string BillingAccount = "billingAccount";

    /// <summary>
    /// API field name for whether the booking is blocked.
    /// </summary>
    internal const string Blocked = "blocked";

    /// <summary>
    /// API field name for the booking project reference.
    /// </summary>
    internal const string BookingProject = "bookingProject";

    /// <summary>
    /// API field name for the counterpart BIC.
    /// </summary>
    internal const string CounterpartBic = "counterpartBic";

    /// <summary>
    /// API field name for the counterpart IBAN.
    /// </summary>
    internal const string CounterpartIban = "counterpartIban";

    /// <summary>
    /// API field name for the booking date.
    /// </summary>
    internal const string Date = "date";

    internal const string DateGt = "date__gt";

    internal const string DateLt = "date__lt";
    
    internal const string DeleteAfterDate = "_deleteAfterDate";
    
    internal const string DeletedBy = "_deletedBy";

    /// <summary>
    /// API field name for the booking description.
    /// </summary>
    internal const string Description = "description";

    /// <summary>
    /// API field name for the unique booking identifier.
    /// </summary>
    internal const string Id = "id";

    internal const string Ordering = "ordering";
    internal const string Org = "org";

    /// <summary>
    /// API field name for the payment difference.
    /// </summary>
    internal const string PaymentDifference = "paymentDifference";

    /// <summary>
    /// API field name for the receiver.
    /// </summary>
    internal const string Receiver = "receiver";

    /// <summary>
    /// API field name for the related invoice references.
    /// </summary>
    internal const string RelatedInvoice = "relatedInvoice";

    internal const string Search = "search";

    /// <summary>API field name for the sphere (area).</summary>
    internal const string Sphere = "sphere";

    /// <summary>
    /// API field name for whether this is a Twingle donation.
    /// </summary>
    internal const string TwingleDonation = "twingleDonation";
}