namespace MCP.EasyVerein.Domain.ValueObjects;

/// <summary>Constants for easyVerein Booking API field names used in JSON serialization.</summary>
public static class BookingFields
{
    /// <summary>API field name for the unique booking identifier.</summary>
    public const string Id = "id";
    /// <summary>API field name for the booking amount.</summary>
    public const string Amount = "amount";
    /// <summary>API field name for the bank account reference.</summary>
    public const string BankAccount = "bankAccount";
    /// <summary>API field name for the billing account reference.</summary>
    public const string BillingAccount = "billingAccount";
    /// <summary>API field name for the booking description.</summary>
    public const string Description = "description";
    /// <summary>API field name for the booking date.</summary>
    public const string Date = "date";
    /// <summary>API field name for the receiver.</summary>
    public const string Receiver = "receiver";
    /// <summary>API field name for the billing ID.</summary>
    public const string BillingId = "billingId";
    /// <summary>API field name for whether the booking is blocked.</summary>
    public const string Blocked = "blocked";
    /// <summary>API field name for the payment difference.</summary>
    public const string PaymentDifference = "paymentDifference";
    /// <summary>API field name for the counterpart IBAN.</summary>
    public const string CounterpartIban = "counterpartIban";
    /// <summary>API field name for the counterpart BIC.</summary>
    public const string CounterpartBic = "counterpartBic";
    /// <summary>API field name for whether this is a Twingle donation.</summary>
    public const string TwingleDonation = "twingleDonation";
    /// <summary>API field name for the booking project reference.</summary>
    public const string BookingProject = "bookingProject";
    /// <summary>API field name for the sphere (area).</summary>
    public const string Sphere = "sphere";
    /// <summary>API field name for the related invoice references.</summary>
    public const string RelatedInvoice = "relatedInvoice";
}
